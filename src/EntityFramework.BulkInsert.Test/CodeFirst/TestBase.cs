using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Calculator.Data;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.BulkInsert.Test.Domain;
using EntityFramework.BulkInsert.Test.Domain.ComplexTypes;
using NUnit.Framework;

namespace EntityFramework.BulkInsert.Test.CodeFirst
{
    public abstract class TestBase<T> where T : DbContext, new()
    {
        [SetUp]
        public virtual void Setup()
        {
            using (var ctx = GetContext())
            {
                if (ctx.Database.Exists())
                {
                    Database.SetInitializer<T>(null);
                }
                else
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    ctx.Database.Initialize(false);
                    sw.Stop();
                    Console.WriteLine("Initializing dbmodel took: {0}ms", sw.Elapsed.TotalMilliseconds);
                }
            }
        }

        protected abstract T GetContext();
        /*
        {
            T ctx = contextName == null 
                ? new T() 
                : (T)Activator.CreateInstance(typeof(T), contextName);

            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = false;
            ctx.Configuration.ProxyCreationEnabled = false;
            ctx.Configuration.ValidateOnSaveEnabled = false;

            return ctx;
        }
        */

        protected static IEnumerable<Page> CreatePages(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return new Page { Title = "title" + i, Content = "content" + i, CreatedAt = DateTime.Now };
            }
        }

        protected static IEnumerable<TestUser> CreateUsers(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return new TestUser
                {
                    CreatedAt = DateTime.Now,
                    FirstName = i + "fn",
                    LastName = "ln" + i,
                    Contact = new Contact { PhoneNumber = "123456", Address = new Address { City = "Tallinn", Country = "Estonia", County = "Harju", PostalCode = "-" } }
                };
            }
        }

        protected static void RunBulkInsert<TItem>(T ctx, IEnumerable<TItem> users, int itemsCount)
        {
            var sw = new Stopwatch();
            sw.Start();
            ctx.BulkInsert(users);
            sw.Stop();
            Console.WriteLine("Bulk insert with {0} items elapsed: {1}ms", itemsCount, TimeSpan.FromTicks(sw.ElapsedTicks).TotalMilliseconds);
        }
    }
}