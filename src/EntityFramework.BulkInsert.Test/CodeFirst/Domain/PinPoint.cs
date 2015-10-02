#if NET45
#if EF6
using System.Data.Entity.Spatial;
#endif
#if EF5
using System.Data.Spatial;
#endif
#endif

namespace EntityFramework.BulkInsert.Test.Domain
{
    public class PinPoint : Entity
    {
        public string Name { get; set; }

#if NET45
        public DbGeography Coordinates { get; set; }
#endif
    }
}