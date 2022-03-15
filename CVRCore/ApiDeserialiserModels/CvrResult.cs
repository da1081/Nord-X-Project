namespace CVRCore.ApiDeserialiserModels
{
    public class CvrResult
    {
        public int? VAT { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Zipcode { get; set; }
        public string? City { get; set; }
        public bool? Protected { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Fax { get; set; }
        public string? Startdate { get; set; }
        public string? Enddate { get; set; }
        public int? Employees { get; set; }
        public string? Addressco { get; set; }
        public int? Industrycode { get; set; }
        public string? Industrydesc { get; set; }
        public int? Companycode { get; set; }
        public string? Companydesc { get; set; }
        public string? Creditstartdate { get; set; }
        public int? Creditstatus { get; set; }
        public bool? Creditbankrupt { get; set; }
        public CompanyOwner[]? Owners { get; set; }
        public ProductionUnit[]? Productionunits { get; set; }
        public int? T { get; set; }
        public int? Version { get; set; }
    }
}
