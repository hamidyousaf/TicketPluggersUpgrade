namespace TP.Upgrade.Application.DTOs
{
    public class MessageSessionDto
    {
        public long OrderId { get; set; }
        public DateTime LastUpdatesOn { get; set; }
        public EventForMessageSessionDto Event { get; set; }
    }
    public class EventForMessageSessionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime EventStartUTC { get; set; }
        public VenueForMessageSessionDto Venue { get; set; }
    }
    public class VenueForMessageSessionDto
    {
        public int Id { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }
}
