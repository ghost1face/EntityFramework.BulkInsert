using System;
#if EF4
using System.ComponentModel.DataAnnotations;
#endif
#if EF6 || EF5
using System.ComponentModel.DataAnnotations.Schema;
#endif
using System.Data.Entity.ModelConfiguration;

namespace Aske.Persistence.Entities
{
    public class CreditReportEntity
    {
        public Guid Id { get; set; }
        public byte[] Timestamp { get; set; }

        public int? AccountsNowDelinq { get; set; }
        public int? AccountsOpenedPast24Months { get; set; }
        public decimal? BankCardsOpenToBuy { get; set; }
        public decimal? PercentBankCardsOver75PercentOfLimit { get; set; }
        public decimal? BankCardUtiliziation { get; set; }
        public decimal? DebtToIncomeRatio { get; set; }
        public int? DelinqPast2Years { get; set; }
        public decimal? DelinqAmount { get; set; }
        public DateTime? EarliestCreditLine { get; set; }
        public int? FicoRangeLow { get; set; }
        public int? FicoRangeHigh { get; set; }
        public int? InquiriesLast6Months { get; set; }
        public int? MonthsSinceLastDelinq { get; set; }
        public int? MonthsSinceLastPublicRecord { get; set; }
        public int? MonthsSinceMostRecentInquiry { get; set; }
        public int? MonthsSinceMostRecentRevolvingDelinq { get; set; }
        public int? MonthsSinceMostRecentBankCardOpened { get; set; }
        public int? MortgageAccounts { get; set; }
        public int? OpenCreditLines { get; set; }
        public int? PublicRecords { get; set; }
        public decimal? TotalCreditBalanceExcludingMortgage { get; set; }
        public decimal? TotalCreditRevolvingBalance { get; set; }
        public decimal? RevolvingUtilizationRate { get; set; }
        public decimal? TotalBankCardCreditLimit { get; set; }
        public int? TotalCreditLines { get; set; }
        public decimal? TotalInstallmentCreditLimit { get; set; }
        public int? RevolvingAccounts { get; set; }
        public int? MonthsSinceMostRecentBankCardDelinq { get; set; }
        public int? PublicRecordBankruptcies { get; set; }
        public int? AccountsEver120DaysPastDue { get; set; }
        public int? ChargeOffsWithin12Months { get; set; }
        public int? CollectionsIn12MonthsExcludingMedical { get; set; }
        public int? TaxLiens { get; set; }
        public int? MonthsSinceLastMajorDerogatory { get; set; }
        public int? SatisfactoryAccounts { get; set; }
        public int? AccountsOpenedPast12Months { get; set; }
        public int? MonthsSinceMostRecentAccountOpened { get; set; }
        public decimal? TotalCreditLimit { get; set; }
        public decimal? TotalCurrentBalanceAllAccounts { get; set; }
        public decimal? AverageCurrentBalanceAllAccounts { get; set; }
        public int? BankCardAccounts { get; set; }
        public int? ActiveBankCardAccounts { get; set; }
        public int? SatisfactoryBankCardAccounts { get; set; }
        public decimal? PercentTradesNeverDelinq { get; set; }
        public int? Accounts90DaysPastDueLast24Months { get; set; }
        public int? Accounts30DaysPastDueLast2Months { get; set; }
        public int? Accounts120DaysPastDueLast2Months { get; set; }
        public int? InstallmentAccounts { get; set; }
        public int? MonthsSinceOldestInstallmentAccountOpened { get; set; }
        public int? CurrentlyActiveRevolvingTrades { get; set; }
        public int? MonthsSinceOldestRevolvingAccountOpened { get; set; }
        public int? MonthsSinceMostRecentRevolvingAccountOpened { get; set; }
        public decimal? TotalRevolvingCreditLimit { get; set; }
        public int? RevolvingTradesWithPositiveBalance { get; set; }
        public int? OpenRevolvingAccounts { get; set; }
        public decimal? TotalCollectionAmountsEverOwed { get; set; }

        public LoanEntity Loan { get; set; }

        public CreditReportEntity()
        {
        }
    }

    public class CreditReportConfig : EntityTypeConfiguration<CreditReportEntity>
    {
        public CreditReportConfig()
        {
            ToTable("CreditReports");

            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Timestamp).IsRowVersion();

            Property(x => x.AccountsNowDelinq).IsOptional();
            Property(x => x.AccountsOpenedPast24Months).IsOptional();
            Property(x => x.BankCardsOpenToBuy).IsOptional();
            Property(x => x.PercentBankCardsOver75PercentOfLimit).IsOptional();
            Property(x => x.BankCardUtiliziation).IsOptional();
            Property(x => x.DebtToIncomeRatio).IsOptional();
            Property(x => x.DelinqPast2Years).IsOptional();
            Property(x => x.DelinqAmount).IsOptional();
            Property(x => x.EarliestCreditLine).IsOptional();
            Property(x => x.FicoRangeLow).IsOptional();
            Property(x => x.FicoRangeHigh).IsOptional();
            Property(x => x.InquiriesLast6Months).IsOptional();
            Property(x => x.MonthsSinceLastDelinq).IsOptional();
            Property(x => x.MonthsSinceLastPublicRecord).IsOptional();
            Property(x => x.MonthsSinceMostRecentInquiry).IsOptional();
            Property(x => x.MonthsSinceMostRecentRevolvingDelinq).IsOptional();
            Property(x => x.MonthsSinceMostRecentBankCardOpened).IsOptional();
            Property(x => x.MortgageAccounts).IsOptional();
            Property(x => x.OpenCreditLines).IsOptional();
            Property(x => x.PublicRecords).IsOptional();
            Property(x => x.TotalCreditBalanceExcludingMortgage).IsOptional();
            Property(x => x.TotalCreditRevolvingBalance).IsOptional();
            Property(x => x.RevolvingUtilizationRate).IsOptional();
            Property(x => x.TotalBankCardCreditLimit).IsOptional();
            Property(x => x.TotalCreditLines).IsOptional();
            Property(x => x.TotalInstallmentCreditLimit).IsOptional();
            Property(x => x.RevolvingAccounts).IsOptional();
            Property(x => x.MonthsSinceMostRecentBankCardDelinq).IsOptional();
            Property(x => x.PublicRecordBankruptcies).IsOptional();
            Property(x => x.AccountsEver120DaysPastDue).IsOptional();
            Property(x => x.ChargeOffsWithin12Months).IsOptional();
            Property(x => x.CollectionsIn12MonthsExcludingMedical).IsOptional();
            Property(x => x.TaxLiens).IsOptional();
            Property(x => x.MonthsSinceLastMajorDerogatory).IsOptional();
            Property(x => x.SatisfactoryAccounts).IsOptional();
            Property(x => x.AccountsOpenedPast12Months).IsOptional();
            Property(x => x.MonthsSinceMostRecentAccountOpened).IsOptional();
            Property(x => x.TotalCreditLimit).IsOptional();
            Property(x => x.TotalCurrentBalanceAllAccounts).IsOptional();
            Property(x => x.AverageCurrentBalanceAllAccounts).IsOptional();
            Property(x => x.BankCardAccounts).IsOptional();
            Property(x => x.ActiveBankCardAccounts).IsOptional();
            Property(x => x.SatisfactoryBankCardAccounts).IsOptional();
            Property(x => x.PercentTradesNeverDelinq).IsOptional();
            Property(x => x.Accounts90DaysPastDueLast24Months).IsOptional();
            Property(x => x.Accounts30DaysPastDueLast2Months).IsOptional();
            Property(x => x.Accounts120DaysPastDueLast2Months).IsOptional();
            Property(x => x.InstallmentAccounts).IsOptional();
            Property(x => x.MonthsSinceOldestInstallmentAccountOpened).IsOptional();
            Property(x => x.CurrentlyActiveRevolvingTrades).IsOptional();
            Property(x => x.MonthsSinceOldestRevolvingAccountOpened).IsOptional();
            Property(x => x.MonthsSinceMostRecentRevolvingAccountOpened).IsOptional();
            Property(x => x.TotalRevolvingCreditLimit).IsOptional();
            Property(x => x.RevolvingTradesWithPositiveBalance).IsOptional();
            Property(x => x.OpenRevolvingAccounts).IsOptional();
            Property(x => x.TotalCollectionAmountsEverOwed).IsOptional();
        }
    }
}
