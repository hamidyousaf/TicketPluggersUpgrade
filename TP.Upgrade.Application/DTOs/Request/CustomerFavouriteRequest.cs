using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class CreateCustomerFavouriteRequest
    {
        public long CustomerId { get; set; }
        public int? EventId { get; set; }
        public int? VenueId { get; set; }
        public int? PerformerId { get; set; }
        public byte FavouriteType { get; set; }
        public bool IsFavourite { get; set; }
    }
    public class DeleteCustomerFavouriteRequest
    {
        [Required]
        public int Id { get; set; }
        public long CustomerId { get; set; }
    }
}
