using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.MappingAPI;

namespace EntityFramework.BulkInsert.Helpers
{
    public class DataTableHelper
    {
        public static DataTable Create<T>(Dictionary<Type, IEntityMap> tableMappings, IEnumerable<T> entities)
        {
            if (tableMappings == null || tableMappings.Count == 0)
            {
                throw new Exception("No table mappings provided.");
            }

            var entityMap = tableMappings.First().Value;
            var firstTableName = entityMap.TableName;
            if (tableMappings.Any(x => x.Value.TableName != firstTableName))
            {
                throw new Exception("All mappings must have same table name.");
            }

            var tableName = string.Format("[{0}].[{1}]", entityMap.Schema, firstTableName);
            var dataTable = new DataTable(tableName);

            var selectors = new Dictionary<Type, Dictionary<string, Func<T, object>>>();

            foreach (var kvp in tableMappings)
            {
                var entityType = kvp.Key;
                selectors[entityType] = new Dictionary<string, Func<T, object>>();

                var tableMapping = kvp.Value;

                var primaryKeys = new List<DataColumn>();
                foreach (var col in tableMapping.Properties)
                {
                    if (!col.IsDiscriminator && !selectors[entityType].ContainsKey(col.ColumnName))
                    {
                        var x = Expression.Parameter(typeof(T), "x");

                        var propNames = col.PropertyName.Split('.');
                        Expression propertyExpression = Expression.PropertyOrField(Expression.Convert(x, entityType), propNames[0]);
                        propertyExpression = propNames.Skip(1).Aggregate(propertyExpression, Expression.PropertyOrField);

                        var expression = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpression, typeof(object)), x);
                        var selector = expression.Compile();
                        selectors[entityType][col.ColumnName] = selector;
                    }

                    if (dataTable.Columns.Contains(col.ColumnName))
                        continue;


                    var dataColumn = new DataColumn(col.ColumnName);

                    if (col.IsDiscriminator)
                    {
                        col.Type = dataColumn.DataType = typeof (int);
                        dataColumn.DefaultValue = col.DefaultValue;
                        dataColumn.AllowDBNull = !col.IsRequired;
                    }
                    else
                    {
                        var propertyInfo = entityType.GetProperty(col.PropertyName, '.');

                        Type dataType;
                        if (propertyInfo.PropertyType.IsNullable(out dataType))
                        {
                            dataColumn.DataType = dataType;
                            dataColumn.AllowDBNull = true;
                        }
                        else
                        {
                            dataColumn.DataType = propertyInfo.PropertyType;
                            dataColumn.AllowDBNull = !col.IsRequired;
                        }

                        col.Type = propertyInfo.PropertyType;
                    }

                    if (col.IsIdentity)
                    {
                        dataColumn.Unique = true;
                        if (col.Type == typeof (int) || col.Type == typeof (long))
                        {
                            dataColumn.AutoIncrement = true;
                        }
                        else if (col.Type == typeof (Guid))
                        {
                            continue;
                        }

                    }
                    else
                    {
                        dataColumn.DefaultValue = col.DefaultValue;
                    }

                    if (col.Type == typeof (string))
                    {
                        dataColumn.MaxLength = col.MaxLength;
                    }
                    if (col.IsPk)
                    {
                        primaryKeys.Add(dataColumn);
                    }
                    dataTable.Columns.Add(dataColumn);
                }
                dataTable.PrimaryKey = primaryKeys.ToArray();
            }


            foreach (var entity in entities)
            {
                var type = entity.GetType();

                DataRow row = dataTable.NewRow();
                
                var tableMapping = tableMappings[type];

                foreach (IPropertyMap col in tableMapping.Properties)
                {
                    object val;
                    if (col.IsDiscriminator)
                    {
                        val = col.DefaultValue;
                    }
                    else
                    {
                        try
                        {
                            val = selectors[type][col.ColumnName](entity);
                        }
                        catch (TargetInvocationException ex)
                        {
                            // complex type property was not set
                            if (ex.InnerException is NullReferenceException)
                            {
                                val = null;
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    if (col.IsIdentity)
                    {
                        continue;
                        if (col.Type == typeof(Guid))
                        {
                            val = Guid.NewGuid();
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (val == null)
                    {
                        row[col.ColumnName] = DBNull.Value;
                    }
                    else
                    {
                        row[col.ColumnName] = val;
                    }
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
