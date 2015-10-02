using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using EntityFramework.BulkInsert.Helpers;
using EntityFramework.BulkInsert.Providers;
#if NET45
using System.Threading.Tasks;
#endif

namespace EntityFramework.BulkInsert.SqlServerCe
{
    public class SqlCeBulkInsertProvider : ProviderBase<SqlCeConnection, SqlCeTransaction>
    {
        protected override SqlCeConnection CreateConnection()
        {
            return new SqlCeConnection(ConnectionString);
        }

        protected override string ConnectionString
        {
            get { return DbConnection.ConnectionString; }
        }

#if NET45

        /// <summary>
        /// Get sql grography object from well known text
        /// </summary>
        /// <param name="wkt">Well known text representation of the value</param>
        /// <param name="srid">The identifier associated with the coordinate system.</param>
        /// <returns></returns>
        public override object GetSqlGeography(string wkt, int srid)
        {
            throw new NotImplementedException();
        }

        public override object GetSqlGeometry(string wkt, int srid)
        {
            throw new NotImplementedException();
        }

        public override Task RunAsync<T>(IEnumerable<T> entities)
        {
            using (var dbConnection = GetConnection())
            {
                dbConnection.Open();

                if ((Options.SqlBulkCopyOptions & SqlBulkCopyOptions.UseInternalTransaction) > 0)
                {
                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        try
                        {
                            Run(entities, (SqlCeConnection)dbConnection, (SqlCeTransaction)transaction);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            if (transaction.Connection != null)
                            {
                                transaction.Rollback();
                            }
                            throw;
                        }
                    }
                }
                else
                {
                    Run(entities, (SqlCeConnection)dbConnection, null);
                }

                return Task.FromResult(0);
            }
        }

#endif

        public override void Run<T>(IEnumerable<T> entities)
        {
            using (var dbConnection = GetConnection())
            {
                dbConnection.Open();

                if ((Options.SqlBulkCopyOptions & SqlBulkCopyOptions.UseInternalTransaction) > 0)
                {
                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        try
                        {
                            Run(entities, (SqlCeConnection)dbConnection, (SqlCeTransaction)transaction);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            if (transaction.Connection != null)
                            {
                                transaction.Rollback();
                            }
                            throw;
                        }
                    }
                }
                else
                {
                    Run(entities, (SqlCeConnection)dbConnection, null);
                }
            }
        }

        private bool IsValidIdentityType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        private void Run<T>(IEnumerable<T> entities, SqlCeConnection connection, SqlCeTransaction transaction)
        {
            bool runIdentityScripts;
            bool keepIdentity = runIdentityScripts = (SqlBulkCopyOptions.KeepIdentity & Options.SqlBulkCopyOptions) > 0;
            var keepNulls = (SqlBulkCopyOptions.KeepNulls & Options.SqlBulkCopyOptions) > 0;

            using (var reader = new MappedDataReader<T>(entities, this))
            {
                var identityCols = reader.Cols.Values.Where(x => x.IsIdentity).ToArray();
                if (identityCols.Length != 1 || !IsValidIdentityType(identityCols[0].Type))
                {
                    runIdentityScripts = false;
                }

                if (keepIdentity && runIdentityScripts)
                {
                    SetIdentityInsert(connection, transaction, reader.TableName, true);
                }

                var colInfos = ColInfos(connection, reader)
                    .Values
                    .Where(x => !x.IsIdentity || keepIdentity)
                    .ToArray();

                using (var cmd = CreateCommand(reader.TableName, connection, transaction))
                {
                    cmd.CommandType = CommandType.TableDirect;
                    using (var rs = cmd.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                        var rec = rs.CreateRecord();
                        int i = 0;
                        long rowsCopied = 0;
                        while (reader.Read())
                        {
                            foreach (var colInfo in colInfos)
                            {
                                var value = reader.GetValue(colInfo.ReaderKey);
                                if (value == null && keepNulls)
                                {
                                    rec.SetValue(colInfo.OrdinalPosition, DBNull.Value);
                                }
                                else
                                {
                                    rec.SetValue(colInfo.OrdinalPosition, value);
                                }
                            }
                            rs.Insert(rec);

                            ++i;
                            if (i == Options.NotifyAfter && Options.Callback != null)
                            {
                                rowsCopied += i;
                                Options.Callback(this, new SqlRowsCopiedEventArgs(rowsCopied));
                                i = 0;
                            }
                        }
                    }
                }

                if (keepIdentity && runIdentityScripts)
                {
                    SetIdentityInsert(connection, transaction, reader.TableName, false);
                }
            }
        }

        public override void Run<T>(IEnumerable<T> entities, SqlCeTransaction transaction)
        {
            Run(entities, (SqlCeConnection)transaction.Connection, transaction);
        }

#if NET45
        public override Task RunAsync<T>(IEnumerable<T> entities, SqlCeTransaction transaction)
        {
            Run(entities, (SqlCeConnection) transaction.Connection, transaction);

            return Task.FromResult(0);
        }
#endif


        private void SetIdentityInsert(SqlCeConnection connection, SqlCeTransaction transaction, string tableName, bool on)
        {
            var commandText = string.Format("SET IDENTITY_INSERT [{0}] {1}", tableName, on ? "ON" : "OFF");
            using (var cmd = CreateCommand(commandText, connection, transaction))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private SqlCeCommand CreateCommand(string commandText, SqlCeConnection connection, SqlCeTransaction transaction)
        {
            var cmd = new SqlCeCommand(commandText, connection);
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }
            return cmd;

        }

        private class ColInfo
        {
            public int OrdinalPosition { get; set; }
            public int ReaderKey { get; set; }
            public bool IsIdentity { get; set; }
        }

        private static Dictionary<string, ColInfo> ColInfos<T>(SqlCeConnection sqlCeConnection, MappedDataReader<T> reader)
        {
            var dtColumns = sqlCeConnection.GetSchema("Columns");

            var colInfos = new Dictionary<string, ColInfo>();
            foreach (DataRow row in dtColumns.Rows)
            {
                var tableName = (string)row.ItemArray[2];
                if (tableName != reader.TableName)
                {
                    continue;
                }

                var columnName = (string)row.ItemArray[3];
                var ordinal = (int)row.ItemArray[4] - 1;

                colInfos[columnName] = new ColInfo { OrdinalPosition = ordinal };
            }

            foreach (var kvp in reader.Cols)
            {
                var colName = kvp.Value.ColumnName;
                if (colInfos.ContainsKey(colName))
                {
                    colInfos[colName].ReaderKey = kvp.Key;
                    colInfos[colName].IsIdentity = kvp.Value.IsIdentity;
                }
            }
            return colInfos;
        }
    }
}
