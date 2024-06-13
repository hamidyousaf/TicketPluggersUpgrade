using TP.Upgrade.Domain.Helpers.Pagination;

namespace TP.Upgrade.Application.DTOs
{
    public class GetEventsByVenueIdNewResponse
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int Id { get; set; }
        public int  VenueId { get; set; }
        public string VenueName { get; set; }
        public string ImageUrl { get; set; }
        public string StartTime { get; set; }
        public string Description { get; set; }
        public DateTime EventDateLocal { get; set; }
        public DateTime EventDateUTC { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
        public int AvailableTicket { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public decimal Distance { get; set; }
    }
}
