namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetRecentHotEventRequest
    {
        public long CustomerId { get; set; }
        public string CountryCode { get; set; } = "GB";
    }
}
