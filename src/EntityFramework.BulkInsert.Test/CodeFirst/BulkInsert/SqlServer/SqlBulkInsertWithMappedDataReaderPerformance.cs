using EntityFramework.BulkInsert.Providers;

namespace EntityFramework.BulkInsert.Test.CodeFirst.BulkInsert.SqlServer
{
    public class SqlBulkInsertWithMappedDataReaderPerformance : PerformanceTestBase<EfSqlBulkInsertProvider>
    {
        protected override string ProviderConnectionType
        {
            get { return "System.Data.SqlClient.SqlConnection"; }
        }

        protected override TestBaseContext GetContext()
        {
            return new TestBaseContext();
        }
    }
}