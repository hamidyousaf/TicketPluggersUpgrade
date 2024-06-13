using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs
{
    public class VenueLite
    {
        public int Id { get; set; }
        public string Venue_ID { get; set; }
        public string VenueName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string StateCode { get; set; }
        public bool IsFavourits { get; set; }
    }
}
