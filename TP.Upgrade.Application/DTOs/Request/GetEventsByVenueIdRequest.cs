namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetEventsByVenueIdRequest
    {
        public long VenueId { get; set; }
        public string Sort { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
