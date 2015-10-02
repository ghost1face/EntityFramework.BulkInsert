using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EntityFramework.BulkInsert.Extensions;
using NUnit.Framework;

namespace EntityFramework.BulkInsert.Test.DbFirst
{
    public class BulkInsertTest : TestBase
    {
        [Test]
        public void InsertBlogs()
        {
            using (var ctx = GetContext())
            {
                InitializeContext();

                var blogs = new List<Blogs>();
                for (int i = 0; i < 10000; ++i)
                {
                    blogs.Add(new Blogs
                    {
                        Name = i + ". name",
                        Url = i + ". url"
                    });
                }

                var sw = new Stopwatch();
                sw.Start();
                ctx.BulkInsert(blogs);
                sw.Stop();
                Console.WriteLine("Bulk insert with {0} items elapsed: {1}ms", 10000, TimeSpan.FromTicks(sw.ElapsedTicks).TotalMilliseconds);
            }
        }

        private void InitializeContext()
        {
            using (var ctx = GetContext())
            {
                var sw = new Stopwatch();
                sw.Start();
                var tmp = ctx.Blogs.Count();
                sw.Stop();
                Console.WriteLine("Initializing dbmodel took: {0}ms", sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}
