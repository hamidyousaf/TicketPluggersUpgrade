using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetFavouriteRequest
    {
        public string SearchString { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; }        //Event,Performer,Venue
        public long CustomerId { get; set; }
    }
}
