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
                string sql = @"IF NOT EXISTS(SELECT * FROM sys.default_constraints WHERE name = 'DF_Audit_AuditUserName')
                BEGIN
                ALTER TABLE [dbo].[Audits] ADD CONSTRAINT [DF_Audit_AuditUserName] DEFAULT (suser_sname()) FOR [AuditUserName]
                END";
                context.Database.ExecuteSqlCommand(sql);
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
