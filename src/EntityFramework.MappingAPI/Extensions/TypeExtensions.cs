using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFramework.MappingAPI.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> typeFieldAccessorLookup;

        static TypeExtensions()
        {
            typeFieldAccessorLookup = new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();
        }

        /// <summary>
        /// Returns a private Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static object GetPrivateFieldValue(this object obj, string propName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type t = obj.GetType();
            FieldInfo fieldInfo = null;
            PropertyInfo propertyInfo = null;
            while (fieldInfo == null && propertyInfo == null && t != null)
            {
                fieldInfo = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    propertyInfo = t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                }

                t = t.BaseType;
            }
            if (fieldInfo == null && propertyInfo == null)
                throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));

            if (fieldInfo != null)
                return fieldInfo.GetValue(obj);

            return propertyInfo.GetValue(obj, null);
        }

        public static TReturn GetFieldValue<T, TReturn>(this T obj, string fieldName)
        {
            var accessor = GetFieldAccessor<T, TReturn>(fieldName);

            return accessor(obj);
        }

        public static TReturn GetFieldValue<TReturn>(this object obj, string fieldName, bool useBaseType)
        {
            var accessor = GetFieldAccessor<TReturn>(obj, fieldName, useBaseType);

            return accessor(obj);
        }

        private static Func<T, TReturn> GetFieldAccessor<T, TReturn>(string fieldName)
        {
            Type type = typeof(T);

            if (typeFieldAccessorLookup.TryGetValue(type, out ConcurrentDictionary<string, object> fieldAccessorLookup)
                && fieldAccessorLookup.TryGetValue(fieldName, out object func))
                return (Func<T, TReturn>)func;

            // add to dictionary
            fieldAccessorLookup = new ConcurrentDictionary<string, object>();
            typeFieldAccessorLookup.TryAdd(type, fieldAccessorLookup);

            ParameterExpression param = Expression.Parameter(type, "arg");
            MemberExpression member = Expression.PropertyOrField(param, fieldName);
            LambdaExpression lambda = Expression.Lambda(typeof(Func<T, TReturn>), member, param);

            var compiled = (Func<T, TReturn>)lambda.Compile();

            fieldAccessorLookup.AddOrUpdate(fieldName, compiled, (key, value) => compiled);

            return compiled;
        }

        private static Func<object, TReturn> GetFieldAccessor<TReturn>(object obj, string fieldName, bool useBaseType)
        {
            Type type = obj.GetType();

            while (useBaseType && type.BaseType != typeof(object))
            {
                type = type.BaseType;
            }

            Type lambdaType = typeof(Func<,>).MakeGenericType(type, typeof(TReturn));

            if (typeFieldAccessorLookup.TryGetValue(type, out ConcurrentDictionary<string, object> fieldAccessorLookup)
                && fieldAccessorLookup.TryGetValue(fieldName, out object func))
                return (Func<object, TReturn>)func;

            // add to dictionary
            fieldAccessorLookup = new ConcurrentDictionary<string, object>();
            typeFieldAccessorLookup.TryAdd(type, fieldAccessorLookup);

            ParameterExpression param = Expression.Parameter(type, "arg");
            MemberExpression member = Expression.PropertyOrField(param, fieldName);
            LambdaExpression lambda = Expression.Lambda(lambdaType, member, param);

            var compiled = (Func<object, TReturn>)lambda.Compile();

            fieldAccessorLookup.AddOrUpdate(fieldName, compiled, (key, value) => compiled);

            return compiled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName, char separator = '.')
        {
            var segments = propertyName.Split(separator);
            object value = null;
            for (int i = 0; i < segments.Length; ++i)
            {
                object tmp = value ?? obj;

                value = tmp.GetType().GetProperty(segments[i]).GetValue(tmp);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this Type type, string propertyName, char separator)
        {
            var segments = propertyName.Split(separator);

            PropertyInfo propertyInfo = null;
            for (int i = 0; i < segments.Length; ++i)
            {
                propertyInfo = type.GetProperty(segments[i], BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                    return null;
                type = propertyInfo.PropertyType;
            }

            return propertyInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentType"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type, out Type argumentType)
        {
            argumentType = null;
            var isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (isNullable)
            {
                argumentType = type.GetGenericArguments()[0];
            }

            return isNullable;
        }

        /// <summary>
        /// Find all derived types from assembly.
        /// If assembly is not given, given type assembly is used.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Type[] GetDerivedTypes(this Type type, Assembly assembly = null)
        {
            return type.GetDerivedTypes(false);
        }

        /// <summary>
        /// Find all derived types from assembly.
        /// If assembly is not given, given type assembly is used.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeItself"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Type[] GetDerivedTypes(this Type type, bool includeItself, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = type.Assembly;
            }

            return assembly
                .GetTypes()
                .Where(t => (includeItself || t != type) && type.IsAssignableFrom(t))
                .ToArray();
        }
    }
}
