using System;
using System.Collections.Generic;
using System.Data.Entity;
using EntityFramework.BulkInsert.Exceptions;
using EntityFramework.BulkInsert.Providers;

namespace EntityFramework.BulkInsert
{
    public class ProviderFactory
    {
        private static Dictionary<string, Func<IEfBulkInsertProvider>> _providers;

        private static readonly object ProviderInitializerLockObject = new object();

        /// <summary>
        /// Registered bulkinsert providers container
        /// </summary>
        private static Dictionary<string, Func<IEfBulkInsertProvider>> Providers
        {
            get
            {
                lock (ProviderInitializerLockObject)
                {
                    if (_providers == null)
                    {
                        _providers = new Dictionary<string, Func<IEfBulkInsertProvider>>();

                        // bundled providers
                        Register<EfSqlBulkInsertProviderWithMappedDataReader>("System.Data.SqlClient.SqlConnection");
                        //Register<EfSqlCeBulkiInsertProvider>("System.Data.SqlServerCe.4.0");
                    }
                }

                return _providers;
            }
        }

        /// <summary>
        /// Register new bulkinsert provider.
        /// Rplaces existing if name matches.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public static void Register<T>(string name) where T : IEfBulkInsertProvider, new()
        {
            Providers[name] = () => new T();
        }

        /*
        public static void Register(Type type, string name)
        {
            // todo - check if type is IEfBulkInsertProvider

            var body = Expression.New(type);

            Expression<Func<IEfBulkInsertProvider>> ex = Expression.Lambda<Func<IEfBulkInsertProvider>>(body);
            var f = ex.Compile();

            Providers[name] = f;
        }
        */

        /// <summary>
        /// Get bulkinsert porvider by connection used in context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEfBulkInsertProvider Get(DbContext context)
        {
            var connectionTypeName = context.Database.Connection.GetType().FullName;
            if (!Providers.ContainsKey(connectionTypeName))
            {
                throw new BulkInsertProviderNotFoundException(connectionTypeName);
            }

            return Providers[connectionTypeName]().SetContext(context);
        }
    }
}
