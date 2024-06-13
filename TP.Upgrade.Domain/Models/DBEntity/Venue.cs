using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Venue : BaseEntity<int>
    {
        [MaxLength(80)]
        public string Venue_ID { get; set; }
        [Required, MaxLength(80)]
        public string VenueName { get; set; } = string.Empty;
        [Required, MaxLength(150)]
        public string Street { get; set; } = string.Empty;
        [Required, MaxLength(80)]
        public string City { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string StateCode { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string CountryCode { get; set; } = string.Empty;
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [MaxLength(40)]
        public string ZipCode { get; set; } = string.Empty;
        [MaxLength(40)]
        public string TimeZone { get; set; } = string.Empty;
        [MaxLength(240)]
        public string VenueURL { get; set; }
        public string VenueImage { get; set; }
        public bool Active { get; set; }
        public bool? IsCustom { get; set; }
        public bool IsDeleted { get; set; }
    }
}