namespace EntityFramework.BulkInsert.Test.Domain
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public CompanySize Size { get; set; }
    }
}
