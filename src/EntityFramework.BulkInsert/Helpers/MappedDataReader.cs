using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using EntityFramework.BulkInsert.Exceptions;
#if NET45
#if EF6
using System.Data.Entity.Spatial;
#else
using System.Data.Spatial;
#endif
#endif

using System.Linq;
using System.Linq.Expressions;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.BulkInsert.Providers;
using EntityFramework.MappingAPI;
using EntityFramework.MappingAPI.Extensions;

namespace EntityFramework.BulkInsert.Helpers
{
    public class MappedDataReader<T> : IDataReader
    {
        private readonly IEnumerator<T> _enumerator;

        public Dictionary<Type, Dictionary<int, Func<T, object>>> Selectors { get; private set; }

        /// <summary>
        /// Property maps by ordinal position
        /// </summary>
        public Dictionary<int, IPropertyMap> Cols { get; private set; } 

        public Dictionary<Type, List<object>> Refs { get; private set; } 

        /// <summary>
        /// Ordinal positions of columns
        /// </summary>
        public Dictionary<string, int> Indexes { get; private set; }

        public int FieldCount { get; private set; }

        public string TableName { get; private set; }

        public string SchemaName { get; private set; }

        public IEfBulkInsertProvider Provider { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="provider"></param>
        public MappedDataReader(IEnumerable<T> enumerable, IEfBulkInsertProvider provider)
        {
            Refs = new Dictionary<Type, List<object>>();

            Provider = provider;

            var baseType = typeof(T);
            var allTypes = baseType.GetDerivedTypes(true);

            //var tableMappings = allTypes.ToDictionary(x => x, Provider.Context.Db);
            var tableMappings = new Dictionary<Type, IEntityMap>();
            foreach (var type in allTypes)
            {
                try
                {
                    var tableMapping = Provider.Context.Db(type);
                    tableMappings[type] = tableMapping;
                }
                catch
                {
                    // todo - catch only EntityTypeNoFoundException when mapping api is upgraded
                    // Maybe these derived types are not used. Throw when invalid type is used while reading.
                }
            }

            if (tableMappings.Count == 0)
            {
                throw new Exception("No table mappings provided.");
            }

            var baseMapping = tableMappings.First().Value;
            var firstTableName = baseMapping.TableName;
            if (tableMappings.Any(x => x.Value.TableName != firstTableName))
            {
                throw new Exception("All mappings must have same table name.");
            }

            TableName = firstTableName;
            SchemaName = baseMapping.Schema;

            Indexes     = new Dictionary<string, int>();
            Cols        = new Dictionary<int, IPropertyMap>();
            Selectors   = new Dictionary<Type, Dictionary<int, Func<T, object>>>();

            _enumerator = enumerable.GetEnumerator();

            int i = 0;
            foreach (var kvp in tableMappings)
            {
                var entityType = kvp.Key;
                Selectors[entityType] = new Dictionary<int, Func<T, object>>();

                var tableMapping = kvp.Value;

                var propertyMaps = tableMapping.Properties
                    .Where(x => !x.Computed && (!x.IsNavigationProperty || x.IsFk));

                foreach (var col in propertyMaps)
                {
                    var currentIndex = i;

                    if (Indexes.ContainsKey(col.ColumnName))
                    {
                        currentIndex = Indexes[col.ColumnName];
                    }
                    else
                    {
                        Cols[currentIndex] = col;
                        Indexes[col.ColumnName] = currentIndex;
                        ++i;
                    }
                     
                    if (col.IsDiscriminator)
                    {
                        var x = Expression.Parameter(baseType, "x");

                        var expression = Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Constant(col.DefaultValue), typeof(object)), x);
                        var selector = expression.Compile();
                        Selectors[entityType][currentIndex] = selector;
                    }
                    else
                    {
                        var x = Expression.Parameter(baseType, "x");

                        var propNames = col.PropertyName.Split('.');
                        Expression propertyExpression = baseType == entityType 
                            ? Expression.PropertyOrField(x, propNames[0])
                            : Expression.PropertyOrField(Expression.Convert(x, entityType), propNames[0]);
                        propertyExpression = propNames.Skip(1).Aggregate(propertyExpression, Expression.PropertyOrField);

                        var expression = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpression, typeof (object)), x);
                        var selector = expression.Compile();
                        Selectors[entityType][currentIndex] = selector;
                    }
                }
            }

            FieldCount = i;
        }

        public void Dispose()
        {
            Selectors = null;
            _enumerator.Dispose();
        }

        private Dictionary<int, Func<T, object>> _currentEntityTypeSelectors; 
        public bool Read()
        {
            var read = _enumerator.MoveNext();
            if (read)
            {
                var t = _enumerator.Current.GetType();
                try
                {
                    _currentEntityTypeSelectors = Selectors[t];
                }
                catch (KeyNotFoundException)
                {
                    throw new EntityTypeNotFoundException(t);
                }
            }
            return read;
        }

        public object GetValue(int i)
        {
            if (_enumerator.Current == null)
            {
                return null;
            }

            try
            {
                object value;
                try
                {
                    value = _currentEntityTypeSelectors[i](_enumerator.Current);

#if NET45
                    var dbgeo = value as DbGeography;
                    if (dbgeo != null)
                    {
                        return Provider.GetSqlGeography(dbgeo.WellKnownValue.WellKnownText, dbgeo.CoordinateSystemId);
                    }

                    var dbgeom = value as DbGeometry;
                    if (dbgeom != null)
                    {
                        return Provider.GetSqlGeometry(dbgeom.WellKnownValue.WellKnownText, dbgeom.CoordinateSystemId);
                    }
#endif
                }
                catch (KeyNotFoundException)
                {
                    // current index is not present in given object type. i.e this column is for some sibling
                    return null;
                }

                // todo - option: copy referenced objects - if it improves performance
                if (Cols[i].IsNavigationProperty)
                {

                    return 0;
                    //var prop = Cols[i].Type.GetProperty(Cols[i].TableMapping.Pk.Prop);
                    //return prop.GetValue(value);
                }

                return value;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public bool IsDBNull(int i)
        {
            return GetValue(i) == null;
        }

        public int GetOrdinal(string name)
        {
            return Indexes[name];
        }

        #region not needed methods

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        object IDataRecord.this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        object IDataRecord.this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public int Depth { get; private set; }
        public bool IsClosed { get; private set; }
        public int RecordsAffected { get; private set; }

        #endregion
    }
}