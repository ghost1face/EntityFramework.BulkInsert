using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using Calculator.Data;
using Calculator.Entities;
using EntityFramework.BulkInsert.Test.Domain;
using EntityFramework.MappingAPI;
using NUnit.Framework;

namespace EntityFramework.BulkInsert.Test.CodeFirst
{
    [TestFixture]
    public class MappingTest : TestBase
    {
        [Test]
        public void Issue1344()
        {
            using (var ctx = new AccrualContext())
            {
                var tableMapping = EfMap.Get<Post>(ctx);
                var columns = tableMapping.Columns;

                Assert.AreEqual(23, columns.Length);

                foreach (var columnMapping in columns)
                {
                    Console.WriteLine("{0}\t{1}", columnMapping.ColumnName, columnMapping.PropertyName);
                }

                AssertColumnName(tableMapping.Columns, "GCRecord", "GcRecord");
            }
        }

        [Test]
        public void TableNames()
        {
            using (var ctx = GetContext())
            {
                InitializeContext();

                var sw = new Stopwatch();
                sw.Restart();
                var dbmapping = EfMap.Get(ctx);
                sw.Start();

                Console.WriteLine("Mapping took: {0}ms", sw.Elapsed.TotalMilliseconds);

                var tableMappings = dbmapping.Tables;

                foreach (var tableMapping in tableMappings)
                {
                    Console.WriteLine("{0}: {1}", tableMapping.TypeFullName, tableMapping.TableName);
                }

                Assert.AreEqual(15, tableMappings.Length);

                AssertTableName<Page>(tableMappings, "Pages");
                AssertTableName<PageTranslations>(tableMappings, "PageTranslations");

                AssertTableName<TestUser>(tableMappings, "Users");

                AssertTableName<MeteringPoint>(tableMappings, "MeteringPoints");

                AssertTableName<EmployeeTPH>(tableMappings, "EmployeeTPHs");
                AssertTableName<AWorkerTPH>(tableMappings, "EmployeeTPHs");
                AssertTableName<ManagerTPH>(tableMappings, "EmployeeTPHs");

                AssertTableName<ContractBase>(tableMappings, "Contracts");
                AssertTableName<Contract>(tableMappings, "Contracts");
                AssertTableName<ContractFixed>(tableMappings, "Contracts");
                AssertTableName<ContractStock>(tableMappings, "Contracts");
                AssertTableName<ContractKomb1>(tableMappings, "Contracts");
                AssertTableName<ContractKomb2>(tableMappings, "Contracts");

                AssertTableName<WorkerTPT>(tableMappings, "WorkerTPTs");
                AssertTableName<ManagerTPT>(tableMappings, "ManagerTPTs");
            }
        }

        private void AssertTableName<T>(IEnumerable<ITableMapping> tableMappings, string tableName)
        {
            Assert.True(tableMappings.Any(x => x.TableName == tableName && x.TypeFullName == typeof(T).FullName));
        }

        private void AssertColumnName(IEnumerable<IColumnMapping> columnMappings, string colName, string propName)
        {
            Console.WriteLine("prop: {0} > col: {1} (index: tbd)", propName, colName);
            var col = columnMappings.FirstOrDefault(x => x.ColumnName == colName && x.PropertyName == propName);
            Assert.IsNotNull(col);
        }

        private ColsAssert Cols(IEnumerable<IColumnMapping> columnMappings)
        {
            return new ColsAssert(columnMappings);
        }

        public class ColAssert
        {
            public ColsAssert And { get; private set; }

            private readonly IColumnMapping _columnMapping;

            public ColAssert(ColsAssert colsAssert, IColumnMapping columnMapping)
            {
                And = colsAssert;
                _columnMapping = columnMapping;
            }

            public ColAssert IsPk(bool isPk = true)
            {
                Assert.AreEqual(isPk, _columnMapping.IsPk);
                return this;
            }

            public ColAssert Prop(string propName)
            {
                Assert.AreEqual(propName, _columnMapping.PropertyName);
                return this;
            }

            public ColAssert IsRefObj(bool isRefObj)
            {
                Assert.AreEqual(isRefObj, _columnMapping.IsRefObject);
                return this;
            }
        }

        public class ColsAssert
        {
            private readonly Dictionary<string, IColumnMapping> _columnMappings;
            public ColsAssert(IEnumerable<IColumnMapping> columnMappings)
            {
                _columnMappings = columnMappings.ToDictionary(x => x.ColumnName);
            }

            public ColAssert Col(string colName)
            {
                Assert.IsTrue(_columnMappings.ContainsKey(colName), "Column mappings does not contain column named '" + colName + "'");
                return new ColAssert(this, _columnMappings[colName]);
            }

            public ColsAssert Count(int count)
            {
                Assert.AreEqual(count, _columnMappings.Count);
                return this;
            }
        }

        [Test]
        public void ColumnNames_ComplexType()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<TestUser>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(9, columns.Length);

                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "FirstName");
                AssertColumnName(columns, "LastName", "LastName");
                AssertColumnName(columns, "Contact_PhoneNumber", "Contact.PhoneNumber");
                AssertColumnName(columns, "Contact_Address_Country", "Contact.Address.Country");
                AssertColumnName(columns, "Contact_Address_County", "Contact.Address.County");
                AssertColumnName(columns, "Contact_Address_City", "Contact.Address.City");
                AssertColumnName(columns, "Contact_Address_PostalCode", "Contact.Address.PostalCode");
                AssertColumnName(columns, "Contact_Address_StreetAddress", "Contact.Address.StreetAddress");
            }
        }

        [Test]
        public void ColumnNames_TPT()
        {
            using (var ctx = GetContext())
            {
                var tableMapping = EfMap.Get<WorkerTPT>(ctx);
                var columns = tableMapping.Columns;

                Cols(columns)
                    .Count(4)
                    .Col("Id")
                        .IsPk().Prop("Id").IsRefObj(false)
                        .And
                    .Col("Name")
                        .IsPk(false).Prop("Name").IsRefObj(false)
                        .And
                    .Col("JobTitle")
                        .IsPk(false).Prop("JobTitle").IsRefObj(false)
                        .And
                    .Col("Boss_Id")
                        .IsPk(false).Prop("Boss").IsRefObj(true);

                tableMapping = EfMap.Get<ManagerTPT>(ctx);
                columns = tableMapping.Columns;

                Cols(columns)
                    .Count(4)
                    .Col("Id")
                        .IsPk().Prop("Id").IsRefObj(false)
                        .And
                    .Col("Name")
                        .IsPk(false).Prop("Name").IsRefObj(false)
                        .And
                    .Col("JobTitle")
                        .IsPk(false).Prop("JobTitle").IsRefObj(false)
                        .And
                    .Col("Rank")
                        .IsPk(false).Prop("Rank").IsRefObj(false);
            }
        }

        [Test]
        public void ColumnNames_TPH_BaseClass()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("EmployeeTPH");
                var tableMapping = EfMap.Get<EmployeeTPH>(ctx);
                var columns = tableMapping.Columns;
                Assert.AreEqual(4, columns.Length);

                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "Name");
                AssertColumnName(columns, "JobTitle", "Title");
                AssertColumnName(columns, "__employeeType", "__employeeType");
            }
        }

        [Test]
        public void ColumnNames_TPH_DerivedType_First()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("WorkerTPH");
                var tableMapping = EfMap.Get<AWorkerTPH>(ctx);
                var columns = tableMapping.Columns;
                Assert.AreEqual(6, columns.Length);

                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "Name");
                AssertColumnName(columns, "JobTitle", "Title");
                AssertColumnName(columns, "BossId", "BossId");
                AssertColumnName(columns, "RefId", "RefId");
                AssertColumnName(columns, "__employeeType", "__employeeType");
            }
        }

        [Test]
        public void ColumnNames_TPH_DerivedType_NotFirst()
        {
            using (var ctx = GetContext())
            {
                Console.WriteLine("ManagerTMP");
                var tableMapping = EfMap.Get<ManagerTPH>(ctx);
                var columns = tableMapping.Columns;
                Assert.AreEqual(6, columns.Length);

                AssertColumnName(columns, "Id", "Id");
                AssertColumnName(columns, "Name", "Name");
                AssertColumnName(columns, "JobTitle", "Title");
                AssertColumnName(columns, "Rank", "Rank");
                AssertColumnName(columns, "RefId1", "RefId");
                AssertColumnName(columns, "__employeeType", "__employeeType");
            }
        }

        [Test]
        public void ColumnNames_Simple()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<Page>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(6, columns.Length);

                AssertColumnName(columns, "PageId", "PageId");
                AssertColumnName(columns, "Title", "Title");
                AssertColumnName(columns, "Content", "Content");
                AssertColumnName(columns, "ParentId", "ParentId");
                AssertColumnName(columns, "CreatedAt", "CreatedAt");
                AssertColumnName(columns, "ModifiedAt", "ModifiedAt");
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractBase()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractBase>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(18, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_Contract()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<Contract>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(18, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractFixed()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractFixed>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(20, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractStock()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractStock>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(20, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractKomb1()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractKomb1>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(23, columns.Length);
            }
        }

        [Test]
        public void ColumnNames_TPH_ContractKomb2()
        {
            using (var ctx = new TestContext())
            {
                var tableMapping = EfMap.Get<ContractKomb2>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(25, columns.Length);
            }
        }
    }
}
