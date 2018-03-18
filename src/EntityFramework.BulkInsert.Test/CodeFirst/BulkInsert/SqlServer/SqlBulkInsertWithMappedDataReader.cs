using EntityFramework.BulkInsert.Providers;

namespace EntityFramework.BulkInsert.Test.CodeFirst.BulkInsert.SqlServer
{
    public class SqlBulkInsertWithMappedDataReader : BulkInsertTestBase<EfSqlBulkInsertProvider, SqlContext>
    {
        protected override string ProviderConnectionType
        {
            get { return "System.Data.SqlClient.SqlConnection"; }
        }

        protected override SqlContext GetContext()
        {
            return new SqlContext();
        }
    }
}
