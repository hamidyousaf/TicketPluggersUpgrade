using System.Diagnostics.Metrics;
using System.Xml.Linq;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.DTOs
{
    public class EventLite
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventStartUTC { get; set; }
        public string StartTime { get; set; }
        public bool IsHotEvent { get; set; }
        public string Venue { get; set; }
        public int VenueId { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public decimal TicketMinPrice { get; set; }
        public decimal TicketMaxPrice { get; set; }
        public int TicketAvailable { get; set; }
        public string ImageURL { get; set; }
        public string SearchName { get; set; }
        public bool IsFavourits { get; set; }
        public string Currency { get; set; }
        public bool IsExpired { get; set; }
        public IList<Product> Tickets { get; set; }
        public int SoldTickects { get; set; }
        public int AvailableTicket { get; set; }
        public short SegmentId { get; set; }
        public int? EventSearchCount { get; set; } = 0;
    }
}