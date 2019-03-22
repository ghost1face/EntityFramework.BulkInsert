using EntityFramework.BulkInsert.Extensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace EntityFramework.BulkInsert.Test.CodeFirst.BulkInsert.SqlServer
{
    [TestFixture]
    public class DatabaseWithDefaultValuesTest
    {
        [Test]
        public void BulkInsert_WithDefaultValues_InsertsDefaultValues()
        {
            using (DatabaseWithDefaultValues ctx = new DatabaseWithDefaultValues())
            {
                ctx.Database.CreateIfNotExists();

                ctx.Database.Initialize(true);

                List<Audit> audits = new List<Audit> { new Audit() };

                ctx.BulkInsert(audits);
            }
        }
    }
}
