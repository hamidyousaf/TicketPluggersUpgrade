namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetFeaturedEventRequest
    {
        public long CustomerId { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public string CountryCode { get; set; } = "GB";
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
