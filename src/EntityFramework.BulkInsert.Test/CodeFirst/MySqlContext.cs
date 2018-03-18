using EntityFramework.BulkInsert.Test.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
#if EF6
using System.Data.Entity.Infrastructure;
#endif
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.BulkInsert.Test.CodeFirst
{
#if EF6
    [DbConfigurationType(typeof(MySqlContextConfig))]
#endif
    public class MySqlContext : TestBaseContext
    {
        public MySqlContext() : base("MySqlTestContext")
        {

        }

#if NET45
        public DbSet<PinPoint> PinPoints { get; set; }
#endif
    }

#if EF6
    public class MySqlContextConfig : DbConfiguration
    {
        public MySqlContextConfig()
        {
            SetProviderServices(
                nameof(global::MySql.Data.MySqlClient),
                new global::MySql.Data.MySqlClient.MySqlProviderServices()
            );

            SetExecutionStrategy(nameof(global::MySql.Data.MySqlClient), () => new DefaultExecutionStrategy());
        }
    }
#endif
}
