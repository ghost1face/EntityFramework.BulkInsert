using System;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DataAccess.EF.Conventions
{
#if EF6
    public class BaseXafConvention : Convention
    {
        public BaseXafConvention()
        {
            Properties<Guid>()
                .Where(p => p.Name == "Oid")
                .Configure(x => x.IsKey());

            Properties()
                .Where(p => p.Name == "GcRecord")
                .Configure(x => x.HasColumnName("GCRecord"));

            Properties<string>()
                .Configure(x => x.HasMaxLength(100));
        }
    }
#endif
}
