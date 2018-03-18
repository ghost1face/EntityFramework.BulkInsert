using EntityFramework.BulkInsert.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
