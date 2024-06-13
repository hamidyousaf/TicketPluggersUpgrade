namespace TP.Upgrade.Application.DTOs
{
    public class CustomerFavouriteLite
    {
        public long CustomerId { get; set; }
        public int? EventId { get; set; }
        public int? VenueId { get; set; }
        public int? PerformerId { get; set; }
        public byte FavouriteType { get; set; }
        public bool NotifyStatus { get; set; }
        public bool IsFavourite { get; set; }
    }
}
