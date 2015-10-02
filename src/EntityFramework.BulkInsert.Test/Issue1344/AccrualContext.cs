#if EF6

using System.Data.Entity;
using Calculator.Data.Configurations;
using Calculator.Entities;
using DataAccess.EF.Conventions;

namespace Calculator.Data
{
    //[DbConfigurationType(typeof(EfDbConfiguration))]
    public class AccrualContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        //public DbSet<StandardDeduction> StandardDeductions { get; set; }

        public AccrualContext()
            : base("TestContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<BaseXafConvention>();
            modelBuilder.Configurations.Add(new PostConfiguration());
            //modelBuilder.Configurations.Add(new StandardDeductionConfiguration());
        } 
    }
}
#endif
