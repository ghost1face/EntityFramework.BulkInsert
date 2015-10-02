using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using EntityFramework.BulkInsert.Helpers;
using EntityFramework.BulkInsert.Test.CodeFirst;
using EntityFramework.BulkInsert.Test.Domain;
using EntityFramework.BulkInsert.Test.Domain.ComplexTypes;
using EntityFramework.MappingAPI;
using EntityFramework.MappingAPI.Extensions;
using NUnit.Framework;
using TestContext = EntityFramework.BulkInsert.Test.CodeFirst.TestContext;

namespace EntityFramework.BulkInsert.Test
{
    public class DataTableHelperTest : TestBase
    {
        [Test]
        public void TableMapping_ColumnsCount_TableWithComplexType()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = ctx.Db<TestUser>();

                var user = new TestUser
                {
                    FirstName = "fn",
                    LastName = "ln",
                    Contact =
                        new Contact
                        {
                            PhoneNumber = "123456",
                            Address =
                                new Address
                                {
                                    City = "Tallinn",
                                    Country = "Estonia",
                                    County = "Harju",
                                    PostalCode = "-"
                                }
                        }
                };

                Console.WriteLine("TestUser table should contain 8 columns");
                var mappings = new Dictionary<Type, IEntityMap> { { typeof(TestUser), tableMapping } };
                using (var dataTable = DataTableHelper.Create(mappings, new[] {user}))
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Console.WriteLine(column.ColumnName);
                    }

                    Assert.AreEqual(8, dataTable.Columns.Count);
                }
            }
        }


        [Test]
        public void TableMapping_ColumnsCount_Contracts()
        {
            using (var ctx = GetContext())
            {
                var allTypes = new[]
                {
                    typeof (ContractBase),
                    typeof (Contract),
                    typeof (ContractFixed),
                    typeof (ContractStock),
                    typeof (ContractKomb1),
                    typeof (ContractKomb2),
                };

                var mappings = allTypes.ToDictionary(x => x, ctx.Db);


                using (var dataTable = DataTableHelper.Create(mappings, new ContractBase[0]))
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Console.WriteLine(column.ColumnName);
                    }

                    Assert.AreEqual(34, dataTable.Columns.Count);
                }
            }
        }


        private IEnumerable<ContractBase> GetContracts(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return new ContractFixed { AvpContractNr = i.ToString(CultureInfo.InvariantCulture), CreatedAt = DateTime.Now };
            }
        }

        [Test, Category("PerformanceTest")]
        public void Create1MilEntities()
        {
            var sw = new Stopwatch();
            sw.Start();
            GetContracts(1000000).ToArray();

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        [Test, Category("PerformanceTest")]
        public void CreateDataTableWith1MilRows()
        {
            using (var ctx = GetContext())
            {
                var allTypes = new[]
                {
                    typeof (ContractBase),
                    typeof (Contract),
                    typeof (ContractFixed),
                    typeof (ContractStock),
                    typeof (ContractKomb1),
                    typeof (ContractKomb2),
                };

                var mappings = allTypes.ToDictionary(x => x, ctx.Db);

                var sw = new Stopwatch();
                sw.Start();
                using (var dataTable = DataTableHelper.Create(mappings, GetContracts(1000000)))
                {
                }

                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}