using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace EntityFramework.BulkInsert.Test.CodeFirst.BulkInsert.SqlServer
{
    [DbConfigurationType(typeof(SqlContextConfig))]
    public class DatabaseWithDefaultValues : DbContext
    {
        public virtual IDbSet<Audit> Audits { get; set; }

        public DatabaseWithDefaultValues()
            : base("TestContext")
        {
            Database.SetInitializer<DatabaseWithDefaultValues>(new DatabaseWithDefaultValuesInitializer());
        }

        private class DatabaseWithDefaultValuesInitializer : IDatabaseInitializer<DatabaseWithDefaultValues>
        {
            public void InitializeDatabase(DatabaseWithDefaultValues context)
            {
                context.Database.ExecuteSqlCommand(
                    "ALTER TABLE [dbo].[Audits] ADD CONSTRAINT [DF_Audit_AuditUserName] DEFAULT (suser_sname()) FOR [AuditUserName];");
            }
        }
    }

    public class Audit
    {
        [Key]
        public long AuditId { get; set; }

        [Required]
        public string AuditUserName { get; set; }
    }
}
