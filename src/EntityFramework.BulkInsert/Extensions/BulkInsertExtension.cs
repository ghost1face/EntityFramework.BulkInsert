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

            var options = new BulkInsertOptions {SqlBulkCopyOptions = sqlBulkCopyOptions};
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

    public static class BulkInsertDefaults
    {
        public static int BatchSize = 5000;
        public static SqlBulkCopyOptions SqlBulkCopyOptions = SqlBulkCopyOptions.Default;
        public static int TimeOut = 30;
        public static int NotifyAfter = 1000;
    }

    public class BulkInsertOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SqlBulkCopyOptions SqlBulkCopyOptions { get; set; }

        /// <summary>
        /// Number of the seconds for the operation to complete before it times out
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Callback event handler. Event is fired after n (value from NotifyAfter) rows have been copied to table where.
        /// </summary>
        public SqlRowsCopiedEventHandler Callback { get; set; }

        /// <summary>
        /// Used with property Callback. Sets number of rows after callback is fired.
        /// </summary>
        public int NotifyAfter { get; set; }

        /// <summary>
        /// If we already have a connection, use it instead of creating a new one.
        /// </summary>
        public IDbConnection Connection { get; set; }

        /// <summary>
        /// If we already have a transaction, use it instead of creating a new one.
        /// </summary>
        public IDbTransaction Transaction { get; set; }

#if !NET40
        /// <summary>
        /// 
        /// </summary>
        public bool EnableStreaming { get; set; }
#endif

        public BulkInsertOptions()
        {
            BatchSize = BulkInsertDefaults.BatchSize;
            SqlBulkCopyOptions = BulkInsertDefaults.SqlBulkCopyOptions;
            TimeOut = BulkInsertDefaults.TimeOut;
            NotifyAfter = BulkInsertDefaults.NotifyAfter;
            Connection = null;
            Transaction = null;
        }
        /*

        /// <summary>
        /// Sets batch size
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public BulkInsertOptions BatchSize(int batchSize)
        {
            BatchSizeValue = batchSize;
            return this;
        }

        /// <summary>
        /// Sets sql bulk copy timeout in seconds
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public BulkInsertOptions TimeOut(int timeout)
        {
            TimeOutValue = timeout;
            return this;
        }

        /// <summary>
        /// Sets SqlBulkCopy options
        /// </summary>
        /// <param name="sqlBulkCopyOptions"></param>
        /// <returns></returns>
        public BulkInsertOptions SqlBulkCopyOptions(SqlBulkCopyOptions sqlBulkCopyOptions)
        {
            SqlBulkCopyOptionsValue = sqlBulkCopyOptions;
            return this;
        }

        /// <summary>
        /// Sets callback method for sql bulk insert
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="interval">Numbers of rows processed before callback is invoked</param>
        /// <returns></returns>
        public BulkInsertOptions Callback(SqlRowsCopiedEventHandler callback, int interval)
        {
            CallbackMethod = callback;
            NotifyAfterValue = interval;

            return this;
        }

#if !NET40
        /// <summary>
        /// Sets batch size
        /// </summary>
        /// <param name="enableStreaming"></param>
        /// <returns></returns>
        public BulkInsertOptions EnableStreaming(bool enableStreaming)
        {
            EnableStreamingValue = enableStreaming;
            return this;
        }
#endif
        */
    }
}
