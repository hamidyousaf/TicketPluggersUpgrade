namespace TP.Upgrade.Application.DTOs.Request
{
    public class SearchEventByEntityRequest
    {
        public string FieldList { get; set; }
        public string Point { get; set; }
        public int RadiusFrom { get; set; }
        public int RadiusTo { get; set; }
        public int Start { get; set; }
        public int Rows { get; set; }
        public string Sort { get; set; }
        public long EntityId { get; set; }
        public int PageSize { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public long CustomerId { get; set; }
        public long CurrencyId { get; set; }
    }
}
