using System;
using System.Diagnostics;
using System.Threading;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.BulkInsert.Providers;
using NUnit.Framework;

namespace EntityFramework.BulkInsert.Test.CodeFirst.BulkInsert
{
    [TestFixture]
    public abstract class PerformanceTestBase<T> : TestBase<TestBaseContext> where T : IEfBulkInsertProvider, new()
    {
        public override void Setup()
        {
            ProviderFactory.Register<T>(ProviderConnectionType);
            base.Setup();
        }

        protected abstract string ProviderConnectionType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagesCount"></param>
        private void BulkInsertPages(int pagesCount)
        {
            using (var ctx = GetContext())
            {
                var pages = CreatePages(pagesCount);
                RunBulkInsert(ctx, pages, pagesCount);
            }
        }

        [Test]
        [Category("PerformanceTest")]
        public void Insert500kPagesInTwoThreads()
        {
            var t = new Thread(() => BulkInsertPages(250000));
            var t2 = new Thread(() => BulkInsertPages(250000));

            var sw = new Stopwatch();
            sw.Start();
            t.Start();
            t2.Start();

            t.Join();
            t2.Join();
            sw.Stop();
            Console.WriteLine("Total:{0}ms", sw.Elapsed.TotalMilliseconds);
        }

        [Test]
        [Category("PerformanceTest")]
        public void Insert500kPagesInFiveThreads()
        {
            var t = new Thread(() => BulkInsertPages(100000));
            var t2 = new Thread(() => BulkInsertPages(100000));
            var t3 = new Thread(() => BulkInsertPages(100000));
            var t4 = new Thread(() => BulkInsertPages(100000));
            var t5 = new Thread(() => BulkInsertPages(100000));

            var sw = new Stopwatch();
            sw.Start();
            t.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();

            t.Join();
            t2.Join();
            t3.Join();
            t4.Join();
            t5.Join();
            sw.Stop();
            Console.WriteLine("Total:{0}ms", sw.Elapsed.TotalMilliseconds);
        }

        [Test]
        [Category("PerformanceTest")]
        public void Insert500kPages()
        {
            BulkInsertPages(500000);
        }

        [Test]
        [Category("PerformanceTest")]
        public void BulkInsertVsAddRange()
        {
            var pagesCount = 1;
            for (int mul = 0; mul < 10; mul++)
            {
                double bulkinsert, addRange;

                using (var ctx = GetContext())
                {
                    var sw = new Stopwatch();
                    sw.Restart();
                    ctx.BulkInsert(CreatePages(pagesCount));
                    sw.Stop();

                    bulkinsert = sw.Elapsed.TotalMilliseconds;
                }

                using (var ctx = GetContext())
                {
                    var sw = new Stopwatch();
                    sw.Restart();
                    ctx.Pages.AddRange(CreatePages(pagesCount));
                    ctx.SaveChanges();
                    sw.Stop();
                    addRange = sw.Elapsed.TotalMilliseconds;
                }
                Console.WriteLine("{0}\t{1}\t{2}", pagesCount, bulkinsert, addRange);

                //Console.WriteLine("{0}\t{1}", pagesCount, bulkinsert);
                pagesCount += 10000;
            }
        }
    }
}
