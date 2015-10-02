using System;
using DataAccess;

namespace Calculator.Entities
{
    public class Post : IntervalEntity
    {
        public Guid? WageTypeId { get; set; }
        public Guid? SubdivisionId { get; set; }
        public Guid? JobCategoryId { get; set; }        
        public int CalculationMonth { get; set; }
        public int CalculationYear { get; set; }        
        public int ActivityMonth { get; set; }
        public int ActivityYear { get; set; }
        public decimal PlannedDays { get; set; }
        public decimal PlannedHours { get; set; }
        public decimal WorkedDays { get; set; }
        public decimal WorkedHours { get; set; }     
        public decimal AppliedPercentage { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMonth { get; set; }
        public int PaymentYear { get; set; }
        public Guid? TaxDeductionTypeId { get; set; }
        public decimal TaxDeductionAmount { get; set; }
        public decimal AppliedTaxDeductionPercentage { get; set; }        
    }
}