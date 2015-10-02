using System.Data.Entity;
using NUnit.Framework;

namespace EntityFramework.BulkInsert.Test.DbFirst
{
    [TestFixture]
    public class TestBase
    {
        [SetUp]
        public void Setup()
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TestContext>());
            //Database.SetInitializer<ef_bulkinsert_db_firstEntities>(null);
        }

        protected DbFirstContext GetContext()
        {
            var ctx = new DbFirstContext();

            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = false;
            ctx.Configuration.ProxyCreationEnabled = false;
            ctx.Configuration.ValidateOnSaveEnabled = false;

            return ctx;
        }
    }
}