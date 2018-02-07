#if EF5 || EF6
using System.Data.Entity;

namespace EntityFramework.BulkInsert.Test.EnumTest
{
    public class EnumTestContext : DbContext
    {
        public EnumTestContext() :
            base("TestContext")
        {

        }

        public DbSet<Company> Companies { get; set; }
    }
}
#endif