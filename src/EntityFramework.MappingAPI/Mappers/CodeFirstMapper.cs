using EntityFramework.MappingAPI.Exceptions;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace EntityFramework.MappingAPI.Mappers
{
    internal class CodeFirstMapper : MapperBase
    {
        public CodeFirstMapper(MetadataWorkspace metadataWorkspace, EntityContainer entityContainer)
            : base(metadataWorkspace, entityContainer)
        {
        }

        protected string GetTableName(EntitySet entitySet)
        {
            return (string)entitySet.MetadataProperties["Table"].Value;
        }

        protected override PrepareMappingRes PrepareMapping(string typeFullName, EdmType edmItem)
        {
            // find existing parent storageEntitySet
            // thp derived types does not have storageEntitySet
            EntitySet storageEntitySet;
            EdmType baseEdmType = edmItem;
            while (!EntityContainer.TryGetEntitySetByName(baseEdmType.Name, false, out storageEntitySet))
            {
                if (baseEdmType.BaseType == null)
                {
                    break;
                }
                baseEdmType = baseEdmType.BaseType;
            }

            if (storageEntitySet == null)
            {
                return null;
            }

            var isRoot = baseEdmType == edmItem;
            if (!isRoot)
            {
                var parent = _entityMaps.Values.FirstOrDefault(x => x.EdmType == baseEdmType);
                // parent table has not been mapped yet
                if (parent == null)
                {
                    throw new ParentNotMappedYetException();
                }
            }

            string tableName = GetTableName(storageEntitySet);

            return new PrepareMappingRes { TableName = tableName, StorageEntitySet = storageEntitySet, IsRoot = isRoot, BaseEdmType = baseEdmType };
        }
    }
}
