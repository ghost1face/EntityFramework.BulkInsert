using EntityFramework.BulkInsert.Extensions;
using EntityFramework.BulkInsert.Test.CodeFirst;
using NUnit.Framework;
using System;
using System.Linq;

namespace EntityFramework.BulkInsert.Test
{
    class InheritanceTests : TestBase<InheritanceIssueContext>
    {
        protected override InheritanceIssueContext GetContext()
        {
            return new InheritanceIssueContext();
        }

#if EF6
        [Test]
        public void BulkInsert_OnlyParent_OneRowInserted()
        {
            using (var ctx = GetContext())
            {
                var parent = new Parent()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                };

                ctx.BulkInsert(new Parent[] { parent });

                Assert.AreEqual(1, ctx.Parents.Count());
            }
        }

        [Test]
        public void BulkInsertChild_ParentExists_OneRowInserted()
        {
            using (var ctx = GetContext())
            {
                var parent = new Parent()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                };
                ctx.Parents.Add(parent);
                ctx.SaveChanges();

                var child = new Child()
                {
                    ID = 1,
                    CreateDate = DateTime.Now,
                };

                ctx.BulkInsert(new Child[] { child });

                Assert.AreEqual(1, ctx.Parents.Count());
                Assert.AreEqual(1, ctx.Childs.Count());
            }
        }

        [Test]
        public void BulkInsertChild_ParentNotExists_OneRowInserted()
        {
            using (var ctx = GetContext())
            {
                var child = new Child()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                    CreateDate = DateTime.Now,
                };

                ctx.BulkInsert(new Child[] { child });

                Assert.AreEqual(1, ctx.Parents.Count());
                Assert.AreEqual(1, ctx.Childs.Count());
            }
        }

        [Test]
        public void BulkInsertParentAndChild_TwoTablesUpdated()
        {
            using (var ctx = GetContext())
            {
                var parent = new Parent()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                };

                var child = new Child()
                {
                    ID = 1,
                    CreateDate = DateTime.Now,
                };

                ctx.BulkInsert(new Parent[] { parent });
                ctx.BulkInsert(new Child[] { child });

                Assert.AreEqual(1, ctx.Parents.Count());
                Assert.AreEqual(1, ctx.Childs.Count());
            }
        }

        [Test]
        public void BulkInsertApChild_OneRowInserted()
        {
            using (var ctx = GetContext())
            {
                var child = new ApChild()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                    CreateDate = DateTime.Now,
                };

                ctx.BulkInsert(new ApChild[] { child });

                Assert.AreEqual(1, ctx.AbstractParents.Count());
                Assert.AreEqual(1, ctx.ApChilds.Count());
            }
        }

        [Test]
        public void BulkInsert_RenamedParent_OneRowInserted()
        {
            using (var ctx = GetContext())
            {
                var parent = new RenamedParent()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                };

                ctx.BulkInsert(new RenamedParent[] { parent });

                Assert.AreEqual(1, ctx.RenamedParents.Count());
            }
        }

        [Test]
        public void BulkInsertChild_RenamedParentExists_OneRowInserted()
        {
            using (var ctx = GetContext())
            {
                var parent = new RenamedParent()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                };
                ctx.RenamedParents.Add(parent);
                ctx.SaveChanges();

                var child = new RenamedChild()
                {
                    ID = 1,
                    CreateDate = DateTime.Now,
                };

                ctx.BulkInsert(new RenamedChild[] { child });

                Assert.AreEqual(1, ctx.RenamedParents.Count());
                Assert.AreEqual(1, ctx.RenamedChilds.Count());
            }
        }

        [Test]
        public void BulkInsertRenamedChild_RenamedParentNotExists_OneRowInserted()
        {
            using (var ctx = GetContext())
            {
                var child = new RenamedChild()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                    CreateDate = DateTime.Now,
                };

                ctx.BulkInsert(new RenamedChild[] { child });

                Assert.AreEqual(1, ctx.RenamedParents.Count());
                Assert.AreEqual(1, ctx.RenamedChilds.Count());
            }
        }

        [Test]
        public void BulkInsertRenamedParentAndChild_TwoTablesUpdated()
        {
            using (var ctx = GetContext())
            {
                var parent = new RenamedParent()
                {
                    ID = 1,
                    Number = "1",
                    IsEmpty = true,
                };

                var child = new RenamedChild()
                {
                    ID = 1,
                    CreateDate = DateTime.Now,
                };

                ctx.BulkInsert(new RenamedParent[] { parent });
                ctx.BulkInsert(new RenamedChild[] { child });

                Assert.AreEqual(1, ctx.RenamedParents.Count());
                Assert.AreEqual(1, ctx.RenamedChilds.Count());
            }
        }

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            using (var ctx = GetContext())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Childs]");
                ctx.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Parents]");

                ctx.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ApChilds]");
                ctx.Database.ExecuteSqlCommand("DELETE FROM [dbo].[AbstractParents]");

                ctx.Database.ExecuteSqlCommand("DELETE FROM [dbo].[RenamedChild]");
                ctx.Database.ExecuteSqlCommand("DELETE FROM [dbo].[RenamedDocument]");
            }
        }
#endif
    }
}
