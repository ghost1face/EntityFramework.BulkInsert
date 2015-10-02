using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.BulkInsert.Helpers;
using EntityFramework.MappingAPI;
using EntityFramework.MappingAPI.Extensions;

namespace EntityFramework.BulkInsert.Providers
{
    public class EfSqlBulkInsertProviderWithDataTable : ProviderBase<SqlConnection, SqlTransaction>
    {
        /*
        public override void Run<T>(IEnumerable<T> entities, BulkInsertOptions options)
        {
            var baseType = typeof(T);
            var allTypes = baseType.GetDerivedTypes(true);

            var neededMappings = allTypes.ToDictionary(x => x, x => EfMap.Get(Context)[x]);

            using (var dataTable = DataTableHelper.Create(neededMappings, entities))
            {
                using (var sqlBulkCopy = new SqlBulkCopy(transaction.Connection, options.SqlBulkCopyOptionsValue, transaction))
                {
                    sqlBulkCopy.BatchSize = options.BatchSizeValue;
                    sqlBulkCopy.BulkCopyTimeout = options.TimeOutValue;
                    if (options.CallbackMethod != null)
                    {
                        sqlBulkCopy.NotifyAfter = options.NotifyAfterValue;
                        sqlBulkCopy.SqlRowsCopied +=  options.CallbackMethod;
                    }

                    sqlBulkCopy.DestinationTableName = dataTable.TableName;
#if !NET40
                    sqlBulkCopy.EnableStreaming = options.EnableStreamingValue;
#endif
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        sqlBulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    sqlBulkCopy.WriteToServer(dataTable);
                }
            }
        }
        */

        public override void Run<T>(IEnumerable<T> entities, SqlTransaction transaction, BulkInsertOptions options)
        {
            var baseType = typeof (T);
            var allTypes = baseType.GetDerivedTypes(true);

            var neededMappings = allTypes.ToDictionary(x => x, x => Context.Db(x));

            using (var dataTable = DataTableHelper.Create(neededMappings, entities))
            {
                using (var sqlBulkCopy = new SqlBulkCopy(transaction.Connection, options.SqlBulkCopyOptions, transaction))
                {
                    sqlBulkCopy.BatchSize = options.BatchSize;
                    sqlBulkCopy.DestinationTableName = dataTable.TableName;
#if !NET40
                    sqlBulkCopy.EnableStreaming = options.EnableStreaming;
#endif

                    foreach (DataColumn col in dataTable.Columns)
                    {
                        sqlBulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    sqlBulkCopy.WriteToServer(dataTable);

                    /*
                    // if there are any reference objects
                    if (tableMapping.Relations.Length > 0)
                    {
                        // can be done only with identity columns
                        var identityColumn = tableMapping.Columns.FirstOrDefault(x => x.IsIdentity);
                        if (identityColumn != null)
                        {
                            var command = transaction.Connection.CreateCommand();
                            command.CommandText = "SELECT max(" + identityColumn.ColumnName + ") from " + dataTable.TableName;
                            command.Transaction = transaction;

                            var res = command.ExecuteScalar();
                            var lastId = long.Parse(res.ToString());
                            var firstId = lastId - dataTable.Rows.Count + 1;



                        }
                    }
                    */
                }
            }
        }

        protected override SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}