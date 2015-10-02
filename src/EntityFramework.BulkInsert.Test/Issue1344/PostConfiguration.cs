using System.Data.Entity.ModelConfiguration;
using Calculator.Entities;

namespace Calculator.Data.Configurations
{
    public class PostConfiguration : EntityTypeConfiguration<Post>
    {
        public PostConfiguration()
        {
            ToTable("Accrual");

            Property(p => p.ResourceId)
                .HasColumnName("PersonalAccount");

            Property(p => p.StartDate)
                .HasColumnName("DateFrom");

            Property(p => p.EndDate)
                .HasColumnName("DateTo");

            Property(p => p.WageTypeId)
                .HasColumnName("Surcharges");

            Property(p => p.SubdivisionId)
                .HasColumnName("CostDepartment");

            Property(p => p.JobCategoryId)
                .HasColumnName("Category");

            Property(p => p.CalculationMonth)
                .HasColumnName("MonthInWhichTheAccrued");

            Property(p => p.CalculationYear)
                .HasColumnName("YearInWhichTheAccrued");

            Property(p => p.ActivityMonth)
                .HasColumnName("MonthForWhichAccrued");

            Property(p => p.ActivityYear)
                .HasColumnName("YearForWhichAccrued");

            Property(p => p.PlannedDays)
                .HasColumnName("DaysGr");

            Property(p => p.PlannedHours)
                .HasColumnName("HoursGr");

            Property(p => p.WorkedDays)
                .HasColumnName("DaysFact");

            Property(p => p.WorkedHours)
                .HasColumnName("HoursFact");

            Property(p => p.AppliedPercentage)
                .HasColumnName("Percent");

            Property(p => p.Amount)
                .HasColumnName("Amount");

            Property(p => p.PaymentMonth)
                .HasColumnName("MonthInWhichPaid");

            Property(p => p.PaymentYear)
                .HasColumnName("YearInWhichPaid");

            Property(p => p.TaxDeductionTypeId)
                .HasColumnName("Deduction");

            Property(p => p.TaxDeductionAmount)
                .HasColumnName("DeductionSumma");

            Property(p => p.AppliedTaxDeductionPercentage)
                .HasColumnName("DeductionPercent");
        }


    }
}
