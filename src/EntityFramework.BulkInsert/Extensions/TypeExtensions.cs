using System;
using System.Linq;
using System.Reflection;

namespace EntityFramework.BulkInsert.Extensions
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Returns a private Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static object GetPrivateFieldValue(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException("obj");
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
#if NET40
                value = tmp.GetType().GetProperty(segments[i]).GetValue(tmp, null);
#else
                value = tmp.GetType().GetProperty(segments[i]).GetValue(tmp);
#endif
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