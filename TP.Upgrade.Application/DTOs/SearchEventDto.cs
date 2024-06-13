namespace TP.Upgrade.Application.DTOs
{
    public class SearchEventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventStartUTC { get; set; }
        public TimeSpan EventStartTime { get; set; }
        public int TicketAvailable { get; set; }
        public decimal TicketMinPrice { get; set; }
        public int VenueId { get; set; }
        public string Venue { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool IsHotEvent { get; set; }
    }
}