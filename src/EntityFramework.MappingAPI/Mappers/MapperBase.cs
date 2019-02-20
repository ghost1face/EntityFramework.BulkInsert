using EntityFramework.MappingAPI.Extensions;
using EntityFramework.MappingAPI.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Data.Entity.Core.Mapping;

namespace EntityFramework.MappingAPI.Mappers
{
    internal abstract class MapperBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected EntityContainer EntityContainer { get; private set; }

        /// <summary>
        /// The magic box
        /// </summary>
        protected readonly MetadataWorkspace MetadataWorkspace;

        /// <summary>
        /// Table mappings dictionary where key is entity type full name.
        /// </summary>
        protected readonly Dictionary<string, EntityMap> _entityMaps = new Dictionary<string, EntityMap>();

        /// <summary>
        /// Primary keys of tables.
        /// Key is table name (in db).
        /// Values is array of entity property names that are primary keys
        /// </summary>
        private readonly Dictionary<string, string[]> _pks = new Dictionary<string, string[]>();

        /// <summary>
        /// Foreign keys map
        /// Key is fk ref
        /// Value is pk ref
        /// </summary>
        private readonly Dictionary<EdmMember, EdmMember> _fks = new Dictionary<EdmMember, EdmMember>();

        private Dictionary<string, TphData> _tphData;

        protected virtual Dictionary<string, TphData> GetTphData()
        {
            var entityContainerMapping = MetadataWorkspace
                .GetItemCollection(DataSpace.CSSpace)[0] as EntityContainerMapping;

            var entitySetMaps = entityContainerMapping.EntitySetMappings;
            //.GetFieldValue<EntityContainerMapping, IReadOnlyCollection<EntitySetBaseMapping>>("EntitySetMaps");
            //.GetPrivateFieldValue("EntitySetMaps");

            var data = new Dictionary<string, TphData>();

            foreach (var entitySetMap in entitySetMaps)
            {
                var props = new List<EdmMember>();
                var navProps = new List<NavigationProperty>();
                var discriminators = new Dictionary<string, object>();

                var typeMappings = entitySetMap.EntityTypeMappings.AsEnumerable();
                //var typeMappings = (IEnumerable<object>)entitySetMap.GetPrivateFieldValue("TypeMappings");

                // make sure that the baseType appear last in the list
                typeMappings = typeMappings
                    .OrderBy(tm => tm.IsOfEntityTypes.Count());
                //typeMappings = typeMappings
                //    .OrderBy(tm => ((IEnumerable<object>)tm.GetPrivateFieldValue("IsOfTypes")).Count());

                foreach (var typeMapping in typeMappings)
                {
                    var types = typeMapping.EntityTypes.Cast<EdmType>();
                    var isOfypes = typeMapping.IsOfEntityTypes.Cast<EdmType>();
                    var mappingFragments = typeMapping.Fragments.AsEnumerable();


                    //var types = (IEnumerable<EdmType>)typeMapping.GetPrivateFieldValue("Types");
                    //var isOfypes = (IEnumerable<EdmType>)typeMapping.GetPrivateFieldValue("IsOfTypes");
                    //var mappingFragments = (IEnumerable<object>)typeMapping.GetPrivateFieldValue("MappingFragments");

                    // if isOfType.length > 0, then it is base type of TPH
                    // must merge properties with siblings
                    foreach (EntityType type in isOfypes)
                    {
                        var identity = type.ToString();
                        if (!data.ContainsKey(identity))
                            data[identity] = new TphData();

                        data[identity].NavProperties = navProps.ToArray();
                        data[identity].Properties = props.ToArray();
                        data[identity].Discriminators = discriminators;
                    }

                    foreach (EntityType type in types)
                    {
                        var identity = type.ToString();
                        if (!data.ContainsKey(identity))
                            data[identity] = new TphData();

                        // type.Properties gets properties including inherited properties
                        var tmp = new List<EdmMember>(type.Properties);

                        foreach (var navProp in type.NavigationProperties)
                        {
                            var associationType = navProp.RelationshipType as AssociationType;
                            if (associationType != null)
                            {
                                // if entity does not contain id property i.e has only reference object for a fk
                                if (associationType.ReferentialConstraints.Count == 0)
                                {
                                    tmp.Add(navProp);
                                }
                            }
                        }

                        data[identity].NavProperties = type.NavigationProperties.ToArray();
                        data[identity].Properties = tmp.ToArray();

                        foreach (var prop in type.Properties)
                        {
                            if (!props.Contains(prop))
                                props.Add(prop);
                        }

                        foreach (var navProp in type.NavigationProperties)
                        {
                            if (!navProps.Contains(navProp))
                                navProps.Add(navProp);
                        }

                        foreach (var fragment in mappingFragments)
                        {
                            //var conditionProperties = (IEnumerable)fragment.GetPrivateFieldValue("m_conditionProperties");
                            var conditionProperties = fragment.Conditions.Cast<ValueConditionMapping>();
                            foreach (var conditionalProperty in conditionProperties)
                            {
                                var columnName = conditionalProperty.Column.Name;
                                var value = conditionalProperty.Value;

                                //var columnName = ((EdmProperty)conditionalProperty.GetPrivateFieldValue("Key")).Name;
                                //var value = conditionalProperty.GetPrivateFieldValue("Value").GetPrivateFieldValue("Value");

                                data[identity].Discriminators[columnName] = value;
                                discriminators[columnName] = value;
                            }
                        }
                    }
                }
            }

            return data;
        }

        //protected virtual Dictionary<string, TphData> GetTphData_2()
        //{
        //    var entitySetMaps = (IEnumerable<object>)MetadataWorkspace
        //            .GetItemCollection(DataSpace.CSSpace)[0]
        //            .GetPrivateFieldValue("EntitySetMaps");

        //    var data = new Dictionary<string, TphData>();

        //    foreach (var entitySetMap in entitySetMaps)
        //    {
        //        var props = new List<EdmMember>();
        //        var navProps = new List<NavigationProperty>();
        //        var discriminators = new Dictionary<string, object>();

        //        var typeMappings = (IEnumerable<object>)entitySetMap.GetPrivateFieldValue("TypeMappings");

        //        // make sure that the baseType appear last in the list
        //        typeMappings = typeMappings
        //            .OrderBy(tm => ((IEnumerable<object>)tm.GetPrivateFieldValue("IsOfTypes")).Count());

        //        foreach (var typeMapping in typeMappings)
        //        {
        //            var types = (IEnumerable<EdmType>)typeMapping.GetPrivateFieldValue("Types");
        //            var isOfypes = (IEnumerable<EdmType>)typeMapping.GetPrivateFieldValue("IsOfTypes");
        //            var mappingFragments = (IEnumerable<object>)typeMapping.GetPrivateFieldValue("MappingFragments");

        //            // if isOfType.length > 0, then it is base type of TPH
        //            // must merge properties with siblings
        //            foreach (EntityType type in isOfypes)
        //            {
        //                var identity = type.ToString();
        //                if (!data.ContainsKey(identity))
        //                    data[identity] = new TphData();

        //                data[identity].NavProperties = navProps.ToArray();
        //                data[identity].Properties = props.ToArray();
        //                data[identity].Discriminators = discriminators;
        //            }

        //            foreach (EntityType type in types)
        //            {
        //                var identity = type.ToString();
        //                if (!data.ContainsKey(identity))
        //                    data[identity] = new TphData();

        //                // type.Properties gets properties including inherited properties
        //                var tmp = new List<EdmMember>(type.Properties);

        //                foreach (var navProp in type.NavigationProperties)
        //                {
        //                    var associationType = navProp.RelationshipType as AssociationType;
        //                    if (associationType != null)
        //                    {
        //                        // if entity does not contain id property i.e has only reference object for a fk
        //                        if (associationType.ReferentialConstraints.Count == 0)
        //                        {
        //                            tmp.Add(navProp);
        //                        }
        //                    }
        //                }

        //                data[identity].NavProperties = type.NavigationProperties.ToArray();
        //                data[identity].Properties = tmp.ToArray();

        //                foreach (var prop in type.Properties)
        //                {
        //                    if (!props.Contains(prop))
        //                        props.Add(prop);
        //                }

        //                foreach (var navProp in type.NavigationProperties)
        //                {
        //                    if (!navProps.Contains(navProp))
        //                        navProps.Add(navProp);
        //                }

        //                foreach (var fragment in mappingFragments)
        //                {
        //                    var conditionProperties = (IEnumerable)fragment.GetPrivateFieldValue("m_conditionProperties");
        //                    foreach (var conditionalProperty in conditionProperties)
        //                    {
        //                        var columnName = ((EdmProperty)conditionalProperty.GetPrivateFieldValue("Key")).Name;
        //                        var value = conditionalProperty.GetPrivateFieldValue("Value").GetPrivateFieldValue("Value");

        //                        data[identity].Discriminators[columnName] = value;
        //                        discriminators[columnName] = value;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return data;
        //}

        public Dictionary<string, TphData> TphData
        {
            get
            {
                if (_tphData != null)
                    return _tphData;

                _tphData = GetTphData();
                return _tphData;
            }
        }

        /// <summary>
        /// Type name and Edm.EntityType map for EF6+
        /// Key is type full name (Also OSpace item fullname and OCSpace identity)
        /// Value is EntityType from CSpace
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, EntityType> GetTypeMappingsEf6()
        {
            return MetadataWorkspace.GetItems(DataSpace.OCSpace)
                    .Select(x => new { identity = x.ToString().Split(':'), edmItem = x.GetPrivateFieldValue("EdmItem") as EntityType })
                    .Where(x => x.edmItem != null)
                    .ToDictionary(x => x.identity[0], x => x.edmItem);
        }


        private Dictionary<string, EntityType> _typeMappings;

        /// <summary>
        /// Type name and Edm.EntityType map.
        /// Key is type full name (Also OSpace item fullname).
        /// Value is EntityType from CSpace.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, EntityType> TypeMappings
        {
            get
            {
                if (_typeMappings != null)
                {
                    return _typeMappings;
                }

                _typeMappings = GetTypeMappingsEf6();

                return _typeMappings;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadataWorkspace"></param>
        /// <param name="entityContainer">Code first or DB first entityContainer</param>
        protected MapperBase(MetadataWorkspace metadataWorkspace, EntityContainer entityContainer)
        {
            MetadataWorkspace = metadataWorkspace;
            EntityContainer = entityContainer;

            var relations = MetadataWorkspace.GetItems(DataSpace.CSpace).OfType<AssociationType>();

            foreach (var associationType in relations)
            {
                foreach (var referentialConstraint in associationType.ReferentialConstraints)
                {
                    for (int i = 0; i < referentialConstraint.ToProperties.Count; ++i)
                    {
                        _fks[referentialConstraint.ToProperties[i]] = referentialConstraint.FromProperties[i];
                    }
                }
            }
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="entitySet"></param>
        /// <returns></returns>
        protected abstract string GetTableName(string typeFullName, EntitySet entitySet);
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        internal EntityMap RegEntity(string typeFullName, string tableName, string schema)
        {
            var entityType = TryGetRefObjectType(typeFullName);

            var entityMapType = typeof(EntityMap<>);
            var genericType = entityMapType.MakeGenericType(entityType);

            var entityMap = (EntityMap)Activator.CreateInstance(genericType);
            entityMap.TypeFullName = typeFullName;
            entityMap.TableName = tableName;
            entityMap.Schema = schema;
            entityMap.Type = entityType;

            _entityMaps.Add(typeFullName, entityMap);

            return entityMap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Type TryGetRefObjectType(string typeFullName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = a.GetType(typeFullName);
                if (t != null)
                    return t;
            }

            return null;
        }

        protected class PrepareMappingRes
        {
            public string TableName { get; set; }
            public EntitySet StorageEntitySet { get; set; }
            public bool IsRoot { get; set; }
            public EdmType BaseEdmType { get; set; }
        }

        protected abstract PrepareMappingRes PrepareMapping(string typeFullName, EdmType edmitem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="edmItem"></param>
        public EntityMap MapEntity(string typeFullName, EdmType edmItem)
        {
            var identity = edmItem.FullName;
            if (!TphData.ContainsKey(identity))
            {
                return null;
            }

            var temp = PrepareMapping(typeFullName, edmItem);
            if (temp == null)
            {
                return null;
            }
            var storageEntitySet = temp.StorageEntitySet;
            var tableName = temp.TableName;
            var isRoot = temp.IsRoot;
            var baseEdmType = temp.BaseEdmType;

            string schema = (string)storageEntitySet.MetadataProperties["Schema"].Value;

            var entityMap = RegEntity(typeFullName, tableName, schema);
            entityMap.IsRoot = isRoot;
            entityMap.EdmType = edmItem;
            entityMap.ParentEdmType = entityMap.IsRoot ? null : baseEdmType;

            _pks[tableName] = storageEntitySet.ElementType.KeyMembers.Select(x => x.Name).ToArray();

            entityMap.IsTph = TphData[identity].Discriminators.Count > 0;

            string prefix = null;
            int i = 0;

            var propertiesToMap = GetPropertiesToMap(entityMap, storageEntitySet.ElementType.Properties);
            foreach (var edmProperty in propertiesToMap)
            {
                MapProperty(entityMap, edmProperty, ref i, ref prefix);
            }

            // create navigation properties
            foreach (var navigationProperty in TphData[identity].NavProperties)
            {
                var associationType = navigationProperty.RelationshipType as AssociationType;
                if (associationType == null || associationType.ReferentialConstraints.Count == 0)
                {
                    continue;
                }

                var to = associationType.ReferentialConstraints[0].ToProperties[0];

                var propertyMap = entityMap.Properties.Cast<PropertyMap>()
                    .FirstOrDefault(x => x.EdmMember == to);

                if (propertyMap != null)
                {
                    propertyMap.NavigationPropertyName = navigationProperty.Name;

                    if (entityMap.Prop(navigationProperty.Name) == null)
                    {
                        var navigationPropertyMap = entityMap.MapProperty(navigationProperty.Name, propertyMap.ColumnName);
                        navigationPropertyMap.IsNavigationProperty = true;
                        navigationPropertyMap.ForeignKeyPropertyName = propertyMap.PropertyName;
                        navigationPropertyMap.ForeignKey = propertyMap;

                        propertyMap.NavigationProperty = navigationPropertyMap;
                    }
                }
            }

            // create discriminators
            foreach (var discriminator in TphData[identity].Discriminators)
            {
                var propertyMap = entityMap.MapDiscriminator(discriminator.Key, discriminator.Value);
                entityMap.AddDiscriminator(propertyMap);
            }

            return entityMap;
        }

        /// <summary>
        /// Gets properties that are ment for given type.
        /// TPH columns are ordered by hierarchy and type name. 
        /// First columns are from base class. Derived types, which name starts with 'A', columns are before type, which name starts with 'B' etc.
        /// So the logic is to include all properties inherited from base types and exclude all already bound properties from siblings.
        /// </summary>
        /// <param name="entityMap"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private IEnumerable<EdmProperty> GetPropertiesToMap(EntityMap entityMap, IEnumerable<EdmProperty> properties)
        {
            if (entityMap.IsRoot)
            {
                return properties;
            }

            var parentEdmType = entityMap.ParentEdmType;

            var include = new List<EdmProperty>();
            var exclude = new List<EdmProperty>();

            while (true)
            {
                var parent = _entityMaps.Values.FirstOrDefault(x => x.EdmType == parentEdmType);
                if (parent == null)
                {
                    break;
                }

                include.AddRange(parent.Properties.Cast<PropertyMap>().Select(x => x.EdmProperty));

                exclude.AddRange(_entityMaps.Values.Where(x => x.ParentEdmType == parentEdmType)
                    .SelectMany(x => x.Properties)
                    .Cast<PropertyMap>()
                    .Select(x => x.EdmProperty));

                parentEdmType = parent.ParentEdmType;
            }

            return properties.Where(edmProperty => include.Contains(edmProperty) || !exclude.Contains(edmProperty)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityMap"></param>
        /// <param name="edmProperty"></param>
        /// <param name="i"></param>
        /// <param name="prefix"></param>
        private void MapProperty(EntityMap entityMap, EdmProperty edmProperty, ref int i, ref string prefix)
        {
            var identity = entityMap.EdmType.FullName;
            var entityMembers = TphData[identity].Properties;
            var columnName = edmProperty.Name;

            if (entityMembers.Length <= i)
            {
                return;
            }
            EdmMember edmMember = entityMembers[i];

            // check if is complex type
            if (string.IsNullOrEmpty(prefix) && edmMember.TypeUsage.EdmType.GetType() == typeof(ComplexType))
            {
                prefix = edmMember.Name;
                ++i;
            }

            string propName;

            if (prefix != null)
            {
                if (columnName.StartsWith(prefix + "_"))
                {
                    propName = columnName.Replace('_', '.');
                }
                else
                {
                    prefix = null;
                    propName = edmMember.Name;
                    ++i;
                }
            }
            else
            {
                propName = entityMembers[i++].Name;
            }

            var propInfo = entityMap.Type.GetProperty(propName, '.');
            if (propInfo == null)
            {
                // entity does not contain such property
                return;
            }

            PropertyMap propertyMap;
            try
            {
                /*
                var mi = SqlProviderServices.Instance.GetType().GetMethod("GetSqlDbType", BindingFlags.NonPublic | BindingFlags.Static);
                var parameterInfos = mi.GetParameters();

                var parameters = new List<object>();
                parameters.Add(edmProperty.TypeUsage);
                parameters.Add(false);
                parameters.Add(SqlVersion);
                parameters.Add(null);
                parameters.Add(null);
                parameters.Add(null);
                parameters.Add(null);

                SqlDbType sqlDbType = (SqlDbType) mi.Invoke(null, parameters.ToArray());
                */

                propertyMap = entityMap.MapProperty(propName, columnName);
                propertyMap.EdmProperty = edmProperty;
                propertyMap.EdmMember = edmMember;
                propertyMap.IsNavigationProperty = edmMember is NavigationProperty;
                propertyMap.IsFk = propertyMap.IsNavigationProperty || _fks.ContainsKey(edmMember);
                if (propertyMap.IsFk && _fks.ContainsKey(edmMember))
                {
                    propertyMap.FkTargetEdmMember = _fks[edmMember];
                    entityMap.AddFk(propertyMap);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Failed to map propName {0} to column {1} on table {2} ({3})",
                    propName,
                    columnName,
                    entityMap.TableName,
                    entityMap.TypeFullName);

                throw new Exception(errorMessage, ex);
            }

            if (_pks[entityMap.TableName].Contains(columnName))
            {
                propertyMap.IsPk = true;
                entityMap.AddPk(propertyMap);
            }

            foreach (var facet in edmProperty.TypeUsage.Facets)
            {
                //System.Data.Entity.Core.Common.DbProviderManifest.ScaleFacetName
                switch (facet.Name)
                {
                    case "SRID":
                        try
                        {
                            propertyMap.SRID = (int?)facet.Value;
                        }
                        catch
                        {
                            // nothing to do
                        }
                        break;
                    case "IsStrict":
                        propertyMap.IsStrict = facet.Value != null && (bool)facet.Value;
                        break;
                    case "Unicode":
                        propertyMap.Unicode = facet.Value != null && (bool)facet.Value;
                        break;
                    case "FixedLength":
                        propertyMap.FixedLength = facet.Value != null && (bool)facet.Value;
                        break;
                    case "Precision":
                        propertyMap.Precision = (byte)facet.Value;
                        break;
                    case "Scale":
                        propertyMap.Scale = (byte)facet.Value;
                        break;
                    case "Nullable":
                        propertyMap.IsRequired = !(bool)facet.Value;
                        break;
                    case "DefaultValue":
                        propertyMap.DefaultValue = facet.Value;
                        break;
                    case "StoreGeneratedPattern":
                        propertyMap.IsIdentity = (StoreGeneratedPattern)facet.Value == StoreGeneratedPattern.Identity;
                        propertyMap.Computed = (StoreGeneratedPattern)facet.Value == StoreGeneratedPattern.Computed;
                        break;
                    case "MaxLength":
                        if (facet.Value == null)
                        {
                            propertyMap.MaxLength = int.MaxValue;
                        }
                        else
                        {
                            int result;
                            var val = facet.Value.ToString();
                            if (!Int32.TryParse(val, out result))
                            {
                                if (val == "Max")
                                {
                                    propertyMap.MaxLength = int.MaxValue;
                                }
                            }
                            propertyMap.MaxLength = result;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void BindForeignKeys()
        {
            var fks = _entityMaps.Values.SelectMany(x => x.Fks);
            var pks = _entityMaps.Values.SelectMany(x => x.Pks);

            // can't use ToDictionary, because tph tables share mappings
            var pkDict = new Dictionary<EdmMember, PropertyMap>();
            foreach (PropertyMap columnMapping in pks)
            {
                pkDict[columnMapping.EdmMember] = columnMapping;
            }

            foreach (PropertyMap fkCol in fks)
            {
                fkCol.FkTargetColumn = pkDict[fkCol.FkTargetEdmMember];
            }
        }
    }
}
