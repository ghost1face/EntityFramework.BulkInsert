using EntityFramework.BulkInsert.Helpers;
using EntityFramework.BulkInsert.Providers;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EntityFramework.BulkInsert.MySql
{
    public class MySqlBulkInsertProvider : ProviderBase<MySqlConnection, MySqlTransaction>
    {
        public MySqlBulkInsertProvider()
        {
            SetProviderIdentifier("MySql.Data.MySqlClient.MySqlConnection");
        }

        public override void Run<T>(IEnumerable<T> entities, MySqlTransaction transaction)
        {
            Run(entities, transaction.Connection, transaction);
        }

#if NET45
        public override Task RunAsync<T>(IEnumerable<T> entities, MySqlTransaction transaction)
        {
            return RunAsync(entities, transaction.Connection, transaction);
        }

        public override object GetSqlGeography(string wkt, int srid)
        {
            throw new NotImplementedException();
        }

        public override object GetSqlGeometry(string wkt, int srid)
        {
            if (!MySqlGeometry.TryParse(wkt, out MySqlGeometry value))
                return null;
            return value;
        }
#endif

        protected override MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
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

        private void Run<T>(IEnumerable<T> entities, MySqlConnection connection, MySqlTransaction transaction)
        {
            bool keepIdentity = (BulkCopyOptions.KeepIdentity & Options.BulkCopyOptions) > 0;
            bool keepNulls = (BulkCopyOptions.KeepNulls & Options.BulkCopyOptions) > 0;

            using (var reader = new MappedDataReader<T>(entities, this))
            {
                var tableEngine = GetTableEngine(connection.Database, reader.TableName, connection);

                var columns = reader.Cols
                    .Where(x => !x.Value.Computed && (!x.Value.IsIdentity || keepIdentity))
                    .ToArray();

                // INSERT INTO [TableName] (column list)
                var insert = new StringBuilder()
                    .Append($" INSERT INTO {reader.TableName} ")
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

                    if (i == Options.BatchSize || i == Options.NotifyAfter)
                    {
                        using (var cmd = CreateCommand(CreateInsertBatchText(insert, rows, tableEngine), connection, transaction))
                            cmd.ExecuteNonQueryAsync();

                        if (Options.Callback != null)
                        {
                            int batches = Options.BatchSize / Options.NotifyAfter;

                            rowsCopied += i;
                            Options.Callback(this, new RowsCopiedEventArgs(rowsCopied));
                        }

                        i = 0;
                        rows.Clear();
                    }
                }

                if (rows.Any())
                {
                    using (var cmd = CreateCommand(CreateInsertBatchText(insert, rows, tableEngine), connection, transaction))
                        cmd.ExecuteNonQuery();
                }
            }
        }

#if NET45
        private async Task RunAsync<T>(IEnumerable<T> entities, MySqlConnection connection, MySqlTransaction transaction)
        {
            bool keepIdentity = (BulkCopyOptions.KeepIdentity & Options.BulkCopyOptions) > 0;
            bool keepNulls = (BulkCopyOptions.KeepNulls & Options.BulkCopyOptions) > 0;

            using (var reader = new MappedDataReader<T>(entities, this))
            {
                var tableEngine = await GetTableEngineAsync(connection.Database, reader.TableName, connection);

                var columns = reader.Cols
                    .Where(x => !x.Value.Computed && (!x.Value.IsIdentity || keepIdentity))
                    .ToArray();

                // INSERT INTO [TableName] (column list)
                var insert = new StringBuilder()
                    .Append($" INSERT INTO {reader.TableName} ")
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

                    if (i == Options.BatchSize || i == Options.NotifyAfter)
                    {
                        using (var cmd = CreateCommand(CreateInsertBatchText(insert, rows, tableEngine), connection, transaction))
                            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (Options.Callback != null)
                        {
                            int batches = Options.BatchSize / Options.NotifyAfter;

                            rowsCopied += i;
                            Options.Callback(this, new RowsCopiedEventArgs(rowsCopied));
                        }

                        i = 0;
                        rows.Clear();
                    }
                }

                if (rows.Any())
                {
                    using (var cmd = CreateCommand(CreateInsertBatchText(insert, rows, tableEngine), connection, transaction))
                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }
#endif

        private void AddParameter(Type type, object value, List<string> values)
        {
            if (type == null
                || type == typeof(string)
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
            else if (type.IsEnum)
            {
                if (value == null)
                {
                    values.Add("NULL");
                }
                else
                {
                    var enumUnderlyingType = type.GetEnumUnderlyingType();
                    values.Add(Convert.ChangeType(value, enumUnderlyingType).ToString());
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
            var cmd = new MySqlCommand(commandText, connection)
            {
                CommandTimeout = Options.TimeOut
            };

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }
            return cmd;
        }

        private string CreateInsertBatchText(string insertHeader, List<string> rows, MySqlEngine engine)
        {
            return GenerateSql(engine, Options.BulkCopyOptions, () => insertHeader + " " + string.Join(",", rows) + ";");
            //return new StringBuilder(insertHeader)
            //    .Append(string.Join(",", rows))
            //    .Append(";")
            //    .AppendLine("COMMIT;")
            //    .ToString();
        }

        private MySqlEngine GetTableEngine(string schemaName, string tableName, MySqlConnection connection)
        {
            using (var cmd = GetTableEngineCommand(schemaName, tableName, connection))
            {
                var engine = cmd.ExecuteScalar();

                Enum.TryParse(engine.ToString(), true, out MySqlEngine tableEngine);

                return tableEngine;
            }
        }

        private async Task<MySqlEngine> GetTableEngineAsync(string schemaName, string tableName, MySqlConnection connection)
        {
            using (var cmd = GetTableEngineCommand(schemaName, tableName, connection))
            {
                var engine = await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                Enum.TryParse(engine.ToString(), true, out MySqlEngine tableEngine);

                return tableEngine;
            }
        }

        private MySqlCommand GetTableEngineCommand(string schemaName, string tableName, MySqlConnection connection)
        {
            var commandText = $@"select engine 
                                from   information_schema.tables 
                                where  table_schema = '{schemaName}'
                                   and table_name = '{tableName}'";

            return CreateCommand(commandText, connection, null);
        }

        private bool IsDateType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsDateType(Nullable.GetUnderlyingType(type));

            return type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }

        private string GenerateSql(MySqlEngine engine, BulkCopyOptions options, Func<string> statementGenerator)
        {
            switch (engine)
            {
                case MySqlEngine.InnoDB:
                    return GenerateInnoDbOptimizations(options, statementGenerator);
                case MySqlEngine.MyISAM:
                    return statementGenerator();
                default:
                    return statementGenerator();
            }
        }

        private string GenerateInnoDbOptimizations(BulkCopyOptions options, Func<string> statementGenerator)
        {
            bool checkConstraints = true;
            if (!options.HasFlag(BulkCopyOptions.Default))
            {
                checkConstraints = options.HasFlag(BulkCopyOptions.CheckConstraints);
            }

            if (!checkConstraints)
            {
                return $@"
SET autocommit=0;
SET unique_checks=0;
SET foreign_key_checks=0;
{statementGenerator()}
COMMIT;
SET unique_checks=1;
SET foreign_key_checks=1;";
            }
            return $@"
SET autocommit=0;
{statementGenerator()}
COMMIT;";
        }
    }
}
