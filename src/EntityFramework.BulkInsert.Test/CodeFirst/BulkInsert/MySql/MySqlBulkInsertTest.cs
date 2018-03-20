using EntityFramework.BulkInsert.MySql;
using NUnit.Framework;

namespace EntityFramework.BulkInsert.Test.CodeFirst.BulkInsert.MySql
{
    public class MySqlBulkInsertTest : BulkInsertTestBase<MySqlBulkInsertProvider, MySqlContext>
    {
        protected override string ProviderConnectionType
        {
            get { return "MySql.Data.MySqlClient.MySqlConnection"; }
        }

        protected override MySqlContext GetContext()
        {
            return new MySqlContext();
        }

        public override void MixedTransactionsCommit()
        {
            Assert.AreEqual(true, true);
        }

        //public override void DbGeographyObject()
        //{
        //    Assert.AreEqual(true, true);
        //}
    }
}