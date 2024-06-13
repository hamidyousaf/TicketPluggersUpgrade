using Microsoft.AspNetCore.Http;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class AddStadiumRequest
    {
        public long VenueId { get; set; }
        public IFormFile File { get; set; }
        public IFormFile ImageFile { get; set; }
        public string ImageURL { get; set; }
        public string description { get; set; }
        public string ZoneNames { get; set; }
    }
}
