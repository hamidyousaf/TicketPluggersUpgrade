namespace TP.Upgrade.Application.DTOs
{
    public class GetEventsBySearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventStartUTC { get; set; }
        public TimeSpan EventStartTime { get; set; }
        public decimal? TicketMinPrice { get; set; }
        public decimal? TicketMaxPrice { get; set; }
        public int? TicketAvailable { get; set; }
        public VenueForGetEventsBySearchDto Venue { get; set; }
    }
    public class VenueForGetEventsBySearchDto
    {
        public int Id { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
    }
}
