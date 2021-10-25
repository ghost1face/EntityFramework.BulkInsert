using EntityFramework.BulkInsert.Test.CodeFirst;
using EntityFramework.BulkInsert.Test.CodeFirst.Domain;
#if EF6
#endif

#if EF4
using System.ComponentModel.DataAnnotations;
#endif

#if EF5
using System.ComponentModel.DataAnnotations.Schema;
#endif

using System.Data.Entity;

namespace EntityFramework.BulkInsert.Test
{
#if EF6
    [DbConfigurationType(typeof(SqlContextConfig))]
#endif
    public class InheritanceIssueContext : DbContext
    {
        public InheritanceIssueContext() : base("InheritanceIssueContext")
        {
        }

        public InheritanceIssueContext(string connectionStringName) : base(connectionStringName)
        {
        }

        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> Childs { get; set; }

        public DbSet<AbstractParent> AbstractParents { get; set; }
        public DbSet<ApChild> ApChilds { get; set; }

        public DbSet<RenamedParent> RenamedParents { get; set; }
        public DbSet<RenamedChild> RenamedChilds { get; set; }
    }
}