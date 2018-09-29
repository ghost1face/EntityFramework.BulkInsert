using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace EntityFramework.MappingAPI.Mappers
{
    internal class TphData
    {
        public EdmMember[] Properties { get; set; }
        public NavigationProperty[] NavProperties { get; set; }

        public Dictionary<string, object> Discriminators = new Dictionary<string, object>();
    }
}
