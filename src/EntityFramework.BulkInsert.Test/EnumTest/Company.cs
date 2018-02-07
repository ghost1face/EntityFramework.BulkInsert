namespace EntityFramework.BulkInsert.Test.EnumTest
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public CompanySize Size { get; set; }
    }
}
