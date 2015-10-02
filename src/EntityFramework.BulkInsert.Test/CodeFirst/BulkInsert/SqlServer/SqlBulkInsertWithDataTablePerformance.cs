using EntityFramework.BulkInsert.Providers;

namespace EntityFramework.BulkInsert.Test.CodeFirst.BulkInsert.SqlServer
{
    public class SqlBulkInsertWithDataTablePerformance : PerformanceTestBase<EfSqlBulkInsertProviderWithDataTable>
    {
        protected override string ProviderConnectionType
        {
            get { return "System.Data.SqlClient.SqlConnection"; }
        }
    }
}