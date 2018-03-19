using EntityFramework.BulkInsert.MySql;

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
    }
}