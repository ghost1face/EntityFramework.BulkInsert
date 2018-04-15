using System.Data;

namespace EntityFramework.BulkInsert
{
    public class BulkInsertOptions
    {
        /// <summary>
        /// Batch size for bulk inserts.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Bulk copy options.
        /// </summary>
        public BulkCopyOptions BulkCopyOptions { get; set; }

        /// <summary>
        /// Number of the seconds for the operation to complete before it times out
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Callback event handler. Event is fired after n (value from NotifyAfter) rows have been copied to table where.
        /// </summary>
        public RowsCopiedEventHandler Callback { get; set; }

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
        /// Enable streaming.
        /// </summary>
        public bool EnableStreaming { get; set; }
#endif

        public BulkInsertOptions()
        {
            BatchSize = BulkInsertDefaults.BatchSize;
            BulkCopyOptions = BulkInsertDefaults.BulkCopyOptions;
            TimeOut = BulkInsertDefaults.TimeOut;
            NotifyAfter = BulkInsertDefaults.NotifyAfter;
            Connection = null;
            Transaction = null;
        }
    }
}
