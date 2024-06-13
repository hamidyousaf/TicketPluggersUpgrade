namespace TP.Upgrade.Application.DTOs
{
    public class GetFavouriteEventDto
    {
        public long Id { get; set; }
        public bool IsFavourite { get; set; }
        public bool NotifyStatus { get; set; }
        public GetFavouriteEventForEventVM Event { get; set; }
    }
    public class GetFavouriteEventForEventVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventStartUTC { get; set;}
        public string StartTime { get; set; }
        public bool IsHotEvent { get; set; }
        public decimal TicketMinPrice { get; set; }
        public int TicketAvailable { get; set; }
        public string? ImageURL { get; set; }
        public GetFavouriteEventForVenueVM Venue { get; set; }
    }
    public class GetFavouriteEventForVenueVM
    {
        public int Id { get; set; }
        public string VenueName { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
    }
}
