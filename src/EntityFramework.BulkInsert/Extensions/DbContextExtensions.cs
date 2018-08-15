#if EF6

using System;
using System.Data.Entity;
using System.Linq.Expressions;

namespace EntityFramework.BulkInsert.Extensions
{
    public static class DbContextExtensions
    {
        private static Delegate originalConnectionStringDelegate;
        private static readonly object syncLock;

        static DbContextExtensions()
        {
            syncLock = new object();
        }

        public static string GetOriginalConnectionString(this DbContext dbContext)
        {
            object internalContext = dbContext.GetFieldValue<DbContext, object>("_internalContext");

            if (originalConnectionStringDelegate == null)
                InitOriginalConnectionStringDelegate(internalContext.GetType());

            return (string)originalConnectionStringDelegate.DynamicInvoke(internalContext);
        }

        private static void InitOriginalConnectionStringDelegate(Type internalContextType)
        {
            if (originalConnectionStringDelegate == null)
            {
                lock (syncLock)
                {
                    if (originalConnectionStringDelegate == null)
                    {
                        Type lambdaType = typeof(Func<,>).MakeGenericType(internalContextType, typeof(string));
                        ParameterExpression param = Expression.Parameter(internalContextType, "arg");
                        MemberExpression member = Expression.PropertyOrField(param, "OriginalConnectionString");
                        LambdaExpression lambda = Expression.Lambda(lambdaType, member, param);

                        originalConnectionStringDelegate = lambda.Compile();
                    }
                }
            }
        }
    }
}
#endif