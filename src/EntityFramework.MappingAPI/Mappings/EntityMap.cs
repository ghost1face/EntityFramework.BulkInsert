using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.MappingAPI.Mappings
{
    /// <summary>
    /// 
    /// </summary>
    internal class EntityMap : IEntityMap
    {
        private readonly Dictionary<string, IPropertyMap> _propertyMaps = new Dictionary<string, IPropertyMap>();
        private readonly List<IPropertyMap> _fks = new List<IPropertyMap>();
        private readonly List<IPropertyMap> _pks = new List<IPropertyMap>();
        private readonly List<IPropertyMap> _discriminators = new List<IPropertyMap>();

        /// <summary>
        /// Entity type full name
        /// </summary>
        public string TypeFullName { get; internal set; }

        /// <summary>
        /// Entity type
        /// </summary>
        public Type Type { get; internal set; }

        /// <summary>
        /// Table name in database
        /// </summary>
        public string TableName { get; internal set; }

        /// <summary>
        /// Database schema
        /// </summary>
        public string Schema { get; internal set; }

        /// <summary>
        /// Is table-per-hierarchy mapping
        /// </summary>
        public bool IsTph { get; internal set; }

        /// <summary>
        /// Is table-per-hierarchy base entity
        /// </summary>
        public bool IsRoot { get; internal set; }

        /// <summary>
        /// Column mappings for table
        /// </summary>
        public IPropertyMap[] Properties
        {
            get { return _propertyMaps.Values.ToArray(); }
        }

        /// <summary>
        /// Foreign key properties
        /// </summary>
        public IPropertyMap[] Fks
        {
            get { return _fks.ToArray(); }
        }

        /// <summary>
        /// Primary key properties
        /// </summary>
        public IPropertyMap[] Pks
        {
            get { return _pks.ToArray(); }
        }

        /// <summary>
        /// Tph entity discriminators
        /// </summary>
        public IPropertyMap[] Discriminators
        {
            get { return _discriminators.ToArray(); }
        }

        /// <summary>
        /// Gets property map by property name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IPropertyMap this[string propertyName]
        {
            get
            {
                if (!_propertyMaps.ContainsKey(propertyName))
                {
                    return null;
                }

                return _propertyMaps[propertyName];
            }
        }

        /// <summary>
        /// Gets property map by property name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IPropertyMap Prop(string propertyName)
        {
            return this[propertyName];
        }

        /// <summary>
        /// Parent DbMapping
        /// </summary>
        //public IDbMapping DbMapping { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        internal EdmType ParentEdmType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal EdmType EdmType { get; set; }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        internal TableMapping(string typeFullName, string tableName, string schema)
        {
            TypeFullName = typeFullName;
            TableName = tableName;
            Schema = schema;

            Type = TryGetRefObjectType();
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public PropertyMap MapProperty(string property, string columnName)
        {
            var cmap = new PropertyMap(property, columnName) { EntityMap = this };
            var propNames = property.Split('.');

            var x = Expression.Parameter(Type, "x");
            Expression propertyExpression = Expression.PropertyOrField(x, propNames[0]);
            propertyExpression = propNames.Skip(1).Aggregate(propertyExpression, Expression.PropertyOrField);

            var expression = Expression.Lambda(Expression.Convert(propertyExpression, typeof(object)), x);
            cmap.Type = propertyExpression.Type;

            var selector = expression.Compile();
            cmap.Selector = selector;

            _propertyMaps.Add(property, cmap);
            return cmap;
        }

        public PropertyMap MapDiscriminator(string name, object defaultValue)
        {
            var cmap = new PropertyMap(name, name) { EntityMap = this, IsDiscriminator = true, DefaultValue = defaultValue };

            var x = Expression.Parameter(Type, "x");
            var expression = Expression.Lambda(Expression.Convert(Expression.Constant(defaultValue), typeof(object)), x);
            var selector = expression.Compile();
            cmap.Selector = selector;

            _propertyMaps.Add(name, cmap);
            return cmap;
        }

        public void AddFk(PropertyMap colMapping)
        {
            _fks.Add(colMapping);
        }

        public void AddPk(PropertyMap colMapping)
        {
            _pks.Add(colMapping);
        }

        public void AddDiscriminator(PropertyMap propertyMap)
        {
            _discriminators.Add(propertyMap);
        }
    }
}
