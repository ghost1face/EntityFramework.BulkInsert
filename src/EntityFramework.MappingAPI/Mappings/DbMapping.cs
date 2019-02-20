using EntityFramework.MappingAPI.Exceptions;
using EntityFramework.MappingAPI.Mappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace EntityFramework.MappingAPI.Mappings
{
    /// <summary>
    /// 
    /// </summary>
    internal class DbMapping
    {
        private readonly Dictionary<string, IEntityMap> _tableMappings = new Dictionary<string, IEntityMap>();
        private readonly string _contextTypeName;
        private readonly DbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public DbMapping(DbContext context)
        {
            _context = context;
            _contextTypeName = context.GetType().FullName;

            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            MetadataWorkspace metadataWorkspace = objectContext.MetadataWorkspace;

            MapperBase mapper;

            EntityContainer entityContainer;
            if (metadataWorkspace.TryGetEntityContainer("CodeFirstDatabase", true, DataSpace.SSpace, out entityContainer))
            {
                mapper = new CodeFirstMapper(metadataWorkspace, entityContainer);
            }
            else
            {
                ReadOnlyCollection<EntityContainer> readOnlyCollection;

                readOnlyCollection = metadataWorkspace.GetItems<EntityContainer>(DataSpace.SSpace);
                entityContainer = readOnlyCollection[0];
                mapper = new DbFirstMapper(metadataWorkspace, entityContainer);
            }

            var typeMappings = mapper.TypeMappings;

            int depth = 0;
            while (true)
            {
                if (depth > 100)
                {
                    throw new Exception("Type mapping has reached unreasonable depth.");
                }

                if (typeMappings.Count == 0)
                {
                    break;
                }

                var nextLevel = new Dictionary<string, EntityType>();

                foreach (var kvp in typeMappings)
                {
                    EntityMap entityMap;
                    try
                    {
                        entityMap = mapper.MapEntity(kvp.Key, kvp.Value);
                    }
                    catch (ParentNotMappedYetException)
                    {
                        nextLevel.Add(kvp.Key, kvp.Value);
                        continue;
                    }

                    if (entityMap == null)
                    {
                        continue;
                    }

                    _tableMappings.Add(kvp.Key, entityMap);
                }

                typeMappings = nextLevel;
                depth++;
            }

            mapper.BindForeignKeys();
        }

        /// <summary>
        /// Tables in database
        /// </summary>
        public IEntityMap[] Tables { get { return _tableMappings.Values.ToArray(); } }

        /// <summary>
        /// Get table mapping by entity type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEntityMap this[Type type]
        {
            get { return this[type.FullName]; }
        }

        /// <summary>
        /// Get table mapping by entity type full name
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public IEntityMap this[string typeFullName]
        {
            get
            {
                if (!_tableMappings.ContainsKey(typeFullName))
                    throw new Exception("Type '" + typeFullName + "' is not found in context '" + _contextTypeName + "'");

                return _tableMappings[typeFullName];
            }
        }

    }
}
