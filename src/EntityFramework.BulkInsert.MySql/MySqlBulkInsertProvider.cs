using EntityFramework.BulkInsert.Helpers;
using EntityFramework.BulkInsert.Providers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;  // TODO: remove this dependency, abstract it
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EntityFramework.BulkInsert.MySql
{
    // TODO: Async Support
    public class MySqlBulkInsertProvider : ProviderBase<MySqlConnection, MySqlTransaction>
    {
        public MySqlBulkInsertProvider()
        {
            SetProviderIdentifier("MySql.Data.MySqlClient.MySqlConnection");
        }

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
                            Run(entities, (MySqlConnection)dbConnection, (MySqlTransaction)transaction);
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
                    Run(entities, (MySqlConnection)dbConnection, null);
                }
            }
        }

        public override void Run<T>(IEnumerable<T> entities, MySqlTransaction transaction)
        {
            Run(entities, transaction.Connection, transaction);
        }

        protected override MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        protected override string ConnectionString => DbConnection.ConnectionString;

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

        private void Run<T>(IEnumerable<T> entities, MySqlConnection connection, MySqlTransaction transaction)
        {
            //bool runIdentityScripts;
            bool keepIdentity = /*runIdentityScripts =*/ (SqlBulkCopyOptions.KeepIdentity & Options.SqlBulkCopyOptions) > 0;
            bool keepNulls = (SqlBulkCopyOptions.KeepNulls & Options.SqlBulkCopyOptions) > 0;

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            using (var reader = new MappedDataReader<T>(entities, this))
            {
                var columns = reader.Cols
                    .Where(x => !x.Value.Computed && (!x.Value.IsIdentity || keepIdentity))
                    .ToArray();

                // INSERT INTO [TableName] (column list)
                var insert = new StringBuilder($"INSERT INTO {reader.TableName} ")
                    .Append("(")
                    .Append(string.Join(",", columns.Select(col => col.Value.ColumnName)))
                    .Append(")")
                    .Append(" VALUES")
                    .ToString();

                int i = 0;
                long rowsCopied = 0;
                var rows = new List<string>();
                while (reader.Read())
                {
                    var values = new List<string>();
                    foreach (var col in columns)
                    {
                        var value = reader.GetValue(col.Key);
                        var type = col.Value.Type;

                        AddParameter(type, value, values);
                    }

                    rows.Add("(" + string.Join(",", values) + ")");

                    i++;
                    if (i == Options.NotifyAfter && Options.Callback != null)
                    {
                        using (var cmd = CreateCommand(CreateInsertBatchText(insert, rows), connection, transaction))
                            cmd.ExecuteNonQuery();

                        rowsCopied += i;
                        Options.Callback(this, new SqlRowsCopiedEventArgs(rowsCopied));
                        i = 0;
                        rows.Clear();
                    }
                }

                if (rows.Any())
                {
                    using (var cmd = CreateCommand(CreateInsertBatchText(insert, rows), connection, transaction))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        private void AddParameter(Type type, object value, List<string> values)
        {
            if (type == typeof(string) 
                || type == typeof(Guid?)
                || type == typeof(Guid))
            {
                if (value == null)
                {
                    values.Add("NULL");
                }
                else
                {
                    values.Add($"'{MySqlHelper.EscapeString(value.ToString())}'");
                }
            }
            else if (IsDateType(type))
            {
                if (value == null)
                {
                    values.Add("NULL");
                }
                else
                {
                    const string dateTimePattern = "yyyy-MM-dd HH:mm:ss.ffffff";
                    if (value is DateTime dt)
                    {
                        values.Add($"'{MySqlHelper.EscapeString(dt.ToString(dateTimePattern))}'");
                    }
                    else if (value is DateTimeOffset dt2)
                    {
                        values.Add($"'{MySqlHelper.EscapeString(dt2.ToString(dateTimePattern))}'");
                    }
                }
            }
            else
            {
                if (value == null)
                {
                    values.Add("NULL");
                }
                else
                {
                    values.Add(value.ToString());
                }
            }
        }

        private MySqlCommand CreateCommand(string commandText, MySqlConnection connection, MySqlTransaction transaction)
        {
            var cmd = new MySqlCommand(commandText, connection);
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }
            return cmd;
        }

        private string CreateInsertBatchText(string insertHeader, List<string> rows)
        {
            return new StringBuilder(insertHeader)
                .Append(string.Join(",", rows))
                .Append(";")
                .ToString();
        }

        private bool IsDateType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsDateType(Nullable.GetUnderlyingType(type));

            return type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }
    }
}
