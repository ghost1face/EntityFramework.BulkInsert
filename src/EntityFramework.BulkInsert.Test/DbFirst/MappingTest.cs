using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EntityFramework.BulkInsert.MappingAPI;
using NUnit.Framework;

namespace EntityFramework.BulkInsert.Test.DbFirst
{
    [TestFixture]
    public class MappingTest : TestBase
    {
        [Test]
        public void TableNames()
        {
            using (var ctx = GetContext())
            {
                var sw = new Stopwatch();
                sw.Start();
                var x = ctx.Blogs.FirstOrDefault();
                sw.Stop();
                Console.WriteLine("Initializing model took: {0}ms", sw.Elapsed.TotalMilliseconds);

                sw.Restart();
                var dbmapping = EfMap.Get(ctx);
                sw.Start();

                Console.WriteLine("Mapping took: {0}ms", sw.Elapsed.TotalMilliseconds);

                var tableMappings = dbmapping.Tables;

                foreach (var tableMapping in tableMappings)
                {
                    Console.WriteLine("{0}: {1}", tableMapping.TypeFullName, tableMapping.TableName);
                }

                Assert.AreEqual(2, tableMappings.Length);

                AssertTableName<Blogs>(tableMappings, "Blogs");
                AssertTableName<Posts>(tableMappings, "Posts");
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

        [Test]
        public void ColumnNames_Blogs()
        {
            using (var ctx = GetContext())
            {
                var tableMapping = EfMap.Get<Blogs>(ctx);

                var columns = tableMapping.Columns;
                Assert.AreEqual(3, columns.Length);

                AssertColumnName(columns, "BlogId", "BlogId");
                AssertColumnName(columns, "Name", "Name");
                AssertColumnName(columns, "Url", "Url");
            }
        }

        [Test]
        public void ColumnNames_Posts()
        {
            using (var ctx = GetContext())
            {
                var tableMapping = EfMap.Get<Posts>(ctx);
                var columns = tableMapping.Columns;
                Assert.AreEqual(4, columns.Length);

                AssertColumnName(columns, "PostId", "PostId");
                AssertColumnName(columns, "Title", "Title");
                AssertColumnName(columns, "Content", "Content");
                AssertColumnName(columns, "BlogId", "BlogId");
            }
        }
    }
}
