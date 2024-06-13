using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class VenueDto
    {
        public VenueDto()
        {
            Id = 0;
            Active = true;
        }
        public long Id { get; set; } = 0;
        [MaxLength(80)]
        [Required]
        public string VenueName { get; set; }
        [MaxLength(80)]
        [Required]
        public string Street { get; set; }
        [MaxLength(80)]
        [Required]
        public string City { get; set; }
        [MaxLength(20)]
        [Required]
        public string StateCode { get; set; }
        [MaxLength(20)]
        [Required]
        public string CountryCode { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [MaxLength(40)]
        [Required]
        public string ZipCode { get; set; }
        [MaxLength(40)]
        [Required]
        public string TimeZone { get; set; }
        public IFormFile VenueImage { get; set; }
        public bool Active { get; set; }
    }
}
