namespace TP.Upgrade.Application.DTOs.Request
{
    public class DeleteZoneRequest
    {
        public long VenueId { get; set; }
        public long[] ZoneId { get; set; }
        public long[] SectionId { get; set; }
    }
}
