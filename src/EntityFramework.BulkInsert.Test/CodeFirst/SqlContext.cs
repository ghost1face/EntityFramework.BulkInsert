using System.Data.Entity;
using EntityFramework.BulkInsert.Test.CodeFirst.Domain;
using EntityFramework.BulkInsert.Test.Domain;
#if EF6
using System.Data.Entity.Infrastructure;
#endif

namespace EntityFramework.BulkInsert.Test.CodeFirst
{
#if EF6
    [DbConfigurationType(typeof(SqlContextConfig))]
#endif
    public class SqlContext : TestBaseContext
    {
#if NET45
        public DbSet<PinPoint> PinPoints { get; set; }
#endif
    }

#if EF6
    public class SqlContextConfig : DbConfiguration
    {
        public SqlContextConfig()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new DefaultExecutionStrategy());
        }
    }
#endif
}
