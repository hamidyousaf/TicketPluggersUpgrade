namespace TP.Upgrade.Application.DTOs.Request
{
    public class UpcommingEventRequest
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string Point { get; set; }
        public int RadiusFrom { get; set; }
        public int RadiusTo { get; set; }
    }
    public class SearchEventByEntity
    {
        public bool Lang { get; set; }
        public string FieldList { get; set; }
        public bool Parking { get; set; }
        public string Point { get; set; }
        public int Radius { get; set; }
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
        public string CountyCode { get; set; }
    }
}
