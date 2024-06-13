using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Response
{
    public class EventOnLoadResponse
    {
        public List<EventOnLoadViewModel> Events { get; set; }
        public string? Poster { get; set; }
        public string[] PopularKeywords { get; set; }
    }
    public class EventOnLoadViewModel
    {
        public int Id { get; set; }
        [MaxLength(80)]
        public string Event_ID { get; set; }
        [MaxLength(240)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
        [MaxLength(40)]
        public string TimeZone { get; set; }
        public int EventStatusId { get; set; }
        public DateTime EventStartUTC { get; set; }
        public DateTime EventEndUTC { get; set; }
        public DateTime EventStart { get; set; }
        public TimeSpan EventStartTime { get; set; }
        public DateTime OnSaleStartDateTime { get; set; }
        public DateTime OnSaleEndDateTime { get; set; }
        [MaxLength(160)]
        public string ImageURL { get; set; }
        public bool IsHotEvent { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int AvailableTicket { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
        public long? VenueID { get; set; }
        public int SegmentId { get; set; }
        public string Country { get; set; }
        public bool isFavourits { get; set; }
    }
}
