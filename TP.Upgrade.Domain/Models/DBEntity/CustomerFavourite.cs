namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class CustomerFavourite
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int? EventId { get; set; }
        public int? VenueId { get; set; }
        public int? PerformerId { get; set; }        // This is Attraction's Id.
        public byte FavouriteType { get; set; }
        public bool NotifyStatus { get; set; }
        public bool IsFavourite { get; set; }
        public DateTime CreatedDate { get; set; }
        public Customer Customer { get; set; }
        public Event? Event { get; set; }
        public Venue? Venue { get; set; }
    }
}
