using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;

#if NET45
#if EF6
using System.Data.Entity.Spatial;
#endif
#if EF5
using System.Data.Spatial;
#endif
#endif

using System.Diagnostics;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.BulkInsert.Helpers;
using EntityFramework.BulkInsert.Providers;
using EntityFramework.BulkInsert.Test.CodeFirst;
using EntityFramework.BulkInsert.Test.Domain;
using EntityFramework.BulkInsert.Test.Domain.ComplexTypes;
using EntityFramework.MappingAPI;
using EntityFramework.MappingAPI.Extensions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EntityFramework.BulkInsert.Test
{
    [TestFixture]
    public class MappedDataReaderTest : TestBase<TestBaseContext>
    {
        public class DummyProvider : IEfBulkInsertProvider
        {
            public IDbConnection GetConnection()
            {
                throw new NotImplementedException();
            }

            public void Run<T>(IEnumerable<T> entities)
            {
                throw new NotImplementedException();
            }

            public void Run<T>(IEnumerable<T> entities, IDbTransaction transaction)
            {
                throw new NotImplementedException();
            }

            public Task RunAsync<T>(IEnumerable<T> entities)
            {
                throw new NotImplementedException();
            }

            public Task RunAsync<T>(IEnumerable<T> entities, IDbTransaction transaction)
            {
                throw new NotImplementedException();
            }

            public object GetSqlGeography(string wkt, int srid)
            {
                throw new NotImplementedException();
            }

            public object GetSqlGeometry(string wkt, int srid)
            {
                throw new NotImplementedException();
            }

            public DbContext Context { get; private set; }
            public BulkInsertOptions Options { get; set; }

            public IEfBulkInsertProvider SetContext(DbContext context)
            {
                Context = context;
                return this;
            }
        }

        private IEfBulkInsertProvider GetDummyProvider(DbContext context)
        {
            return new DummyProvider().SetContext(context);
        }

        [Test]
        public void Performance()
        {
            var sw = new Stopwatch();
            var swv = new Stopwatch();

            using (var ctx = new TestBaseContext())
            {
                ctx.Database.Initialize(false);
                sw.Restart();
                using (var reader = new MappedDataReader<Page>(CreatePages(1000000), GetDummyProvider(ctx)))
                {
                    Console.WriteLine("Construct {0}ms", sw.Elapsed.TotalMilliseconds);
                    while (reader.Read())
                    {
                        foreach (var col in reader.Cols)
                        {
                            swv.Start();
                            var value = reader.GetValue(col.Key);
                            swv.Stop();
                        }
                    }
                }
                sw.Stop();
                Console.WriteLine("Elapsed {0}ms. Getting values took {1}ms", sw.Elapsed.TotalMilliseconds, swv.Elapsed.TotalMilliseconds);
            }
        }

        [Test]
        public void SimpleTableReader()
        {
            using (var ctx = new TestBaseContext())
            {
                using (var reader = new MappedDataReader<Page>(new[] { new Page { Title = "test" } }, GetDummyProvider(ctx)))
                {
                    Assert.AreEqual(6, reader.FieldCount);
                }
            }
        }

        [Test]
        public void ComplexTypeReader()
        {
            var user = new TestUser
            {
                Contact = new Contact { Address = new Address { City = "Tallinn", Country = "Estonia"}, PhoneNumber = "1234567"},
                FirstName = "Max",
                LastName = "Lego",
                Id = Guid.NewGuid()
            };
            var emptyUser = new TestUser();

            using (var ctx = new TestBaseContext())
            {
                using (var reader = new MappedDataReader<TestUser>(new[] { user, emptyUser }, GetDummyProvider(ctx)))
                {
                    Assert.AreEqual(11, reader.FieldCount);
                    
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            Console.WriteLine("{0}: {1}", i, reader.GetValue(i));
                        }
                    }
                }
            }
        }

        protected override TestBaseContext GetContext()
        {
            return new TestBaseContext();
        }
    }
}