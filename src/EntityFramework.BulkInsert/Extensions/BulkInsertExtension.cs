using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EntityFramework.BulkInsert.Extensions
{
    public static class BulkInsertExtension
    {

#if NET45

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="options"></param>
        public static Task BulkInsertAsync<T>(this DbContext context, IEnumerable<T> entities, BulkInsertOptions options)
        {
            var bulkInsert = ProviderFactory.Get(context);
            bulkInsert.Options = options;
            return bulkInsert.RunAsync(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public static Task BulkInsertAsync<T>(this DbContext context, IEnumerable<T> entities, int? batchSize = null)
        {
            return context.BulkInsertAsync(entities, SqlBulkCopyOptions.Default, batchSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="sqlBulkCopyOptions"></param>
        /// <param name="batchSize"></param>
        public static Task BulkInsertAsync<T>(this DbContext context, IEnumerable<T> entities, SqlBulkCopyOptions sqlBulkCopyOptions, int? batchSize = null)
        {
            var options = new BulkInsertOptions { SqlBulkCopyOptions = sqlBulkCopyOptions };
            if (batchSize.HasValue)
            {
                options.BatchSize = batchSize.Value;
            }
            return context.BulkInsertAsync(entities, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        /// <param name="sqlBulkCopyOptions"></param>
        /// <param name="batchSize"></param>
        public static Task BulkInsertAsync<T>(this DbContext context, IEnumerable<T> entities, IDbTransaction transaction, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, int? batchSize = null)
        {
            var options = new BulkInsertOptions { SqlBulkCopyOptions = sqlBulkCopyOptions };
            if (transaction != null)
            {
                options.Connection = transaction.Connection;
                options.Transaction = transaction;
            }
            if (batchSize.HasValue)
            {
                options.BatchSize = batchSize.Value;
            }
            return context.BulkInsertAsync(entities, options);
        }

#endif

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="options"></param>
        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities, BulkInsertOptions options)
        {
            var bulkInsert = ProviderFactory.Get(context);
            bulkInsert.Options = options;
            bulkInsert.Run(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="batchSize"></param>
        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities, int? batchSize = null)
        {
            context.BulkInsert(entities, SqlBulkCopyOptions.Default, batchSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="sqlBulkCopyOptions"></param>
        /// <param name="batchSize"></param>
        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities, SqlBulkCopyOptions sqlBulkCopyOptions, int? batchSize = null)
        {

            var options = new BulkInsertOptions { SqlBulkCopyOptions = sqlBulkCopyOptions };
            if (batchSize.HasValue)
            {
                options.BatchSize = batchSize.Value;
            }
            context.BulkInsert(entities, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        /// <param name="sqlBulkCopyOptions"></param>
        /// <param name="batchSize"></param>
        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities, IDbTransaction transaction, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, int? batchSize = null)
        {
            var options = new BulkInsertOptions { SqlBulkCopyOptions = sqlBulkCopyOptions };
            if (transaction != null)
            {
                options.Connection = transaction.Connection;
                options.Transaction = transaction;
            }

            if (batchSize.HasValue)
            {
                options.BatchSize = batchSize.Value;
            }
            context.BulkInsert(entities, options);
        }

        /*
        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities,
            Func<BulkInsertOptions, BulkInsertOptions> options)
        {
            var bulkInsert = ProviderFactory.Get(context);
            bulkInsert.Run(entities, options(new BulkInsertOptions()));
        }
         * */
    }
}
