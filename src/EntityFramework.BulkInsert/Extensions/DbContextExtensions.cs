#if EF6

using System;
using System.Data.Entity;
using System.Linq.Expressions;

namespace EntityFramework.BulkInsert.Extensions
{
    public static class DbContextExtensions
    {
        public static string GetOriginalConnectionString(this DbContext dbContext)
        {
            object internalContext = dbContext.GetFieldValue<DbContext, object>("_internalContext");

            Type internalType = internalContext.GetType();
            Type lambdaType = typeof(Func<,>).MakeGenericType(internalType, typeof(string));
            ParameterExpression param = Expression.Parameter(internalType, "arg");
            MemberExpression member = Expression.PropertyOrField(param, "OriginalConnectionString");
            LambdaExpression lambda = Expression.Lambda(lambdaType, member, param);

            return (string)lambda.Compile().DynamicInvoke(internalContext);
        }

        private static Type GetBaseType(object obj)
        {
            Type type = obj.GetType();

            while (type.BaseType != typeof(object))
            {
                type = type.BaseType;
            }

            return type;
        }
    }
}
#endif