using System;

namespace EntityFramework.BulkInsert.Test.Domain
{
    public abstract class ContractBase : Entity, ICreatedAt, IModifiedAt
    {
        public string ContractNr { get; set; }
        public string AvpContractNr { get; set; }

        public int MeteringPointId { get; set; }
        public virtual MeteringPoint MeteringPoint { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ContractSignedAt { get; set; }

        /// <summary>
        /// Utc time of contract start date. Readonly
        /// </summary>
        public DateTime StartDateUtc { get; protected set; }

        /// <summary>
        /// Utc time of contract end date. Readonly
        /// </summary>
        public DateTime? EndDateUtc { get; protected set; }

        public int ClientId { get; set; }

        public decimal? FixedPrice { get; set; }

        public int? PackageId { get; set; }
        public string PackageName { get; set; }

        public bool Validated { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? LastPricesCalculatedAt { get; set; }

        protected ContractBase()
        {
            StartDate = DateTime.Now;
            StartDateUtc = DateTime.Now;
            CreatedAt = DateTime.Now;
        }
    }

    public class Contract : ContractBase
    {

    }

    public class ContractFixed : ContractBase
    {
        public int PackageFixedId { get; set; }

        public string PricesJson { get; set; }
    }

    public class ContractStock : ContractBase
    {
        public decimal? Margin { get; set; }

        public int PackageStockId { get; set; }
    }

    public class ContractKomb1 : ContractBase
    {
        public int PackageKomb1Id { get; set; }

        public decimal Base { get; set; }
        public decimal? StockMargin { get; set; }
        public string FixPricesJson { get; set; }

        public int SubPackageId { get; set; }
    }

    public class ContractKomb2 : ContractBase
    {
        public int PackageKomb2Id { get; set; }
        
        public decimal? Part1Margin { get; set; }
        public string Part1PricesJson { get; set; }
        
        public int Part1SubPackageId { get; set; }

        public decimal? Part2Margin { get; set; }
        public string Part2PricesJson { get; set; }
        
        public int Part2SubPackageId { get; set; }
    }
}
