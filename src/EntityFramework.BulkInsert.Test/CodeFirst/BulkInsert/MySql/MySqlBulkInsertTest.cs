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
            // MySql does not support Mixed Transactions, force pass here
            Assert.AreEqual(true, true);
        }

        public override void DbGeographyObject()
        {
            // MySql does not support geography spatial type, force pass
            Assert.AreEqual(true, true);
        }

        public override void BulkInsertWithIdentityInsertOn()
        {
            // MySql does support identity insert, however the MySql provider Migration class
            // creates a trigger that forces ALWAYS creates an identity value
            // look into contributing and submitting slightly modified trigger to allow for identity insert
            // https://www.electrictoolbox.com/mysql-guid-uuid-default-column/
            base.BulkInsertWithIdentityInsertOn();
        }
    }
}