using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs
{
    public class GetFavouriteVenueDto
    {
        public long Id { get; set; }
        public bool NotifyStatus { get; set; }
        public bool IsFavourite { get; set; }
        public GetFavouriteVenueVM Venue { get; set; }
    }
    public class GetFavouriteVenueVM
    {
        public int Id { get; set; }
        public string Venue_ID { get; set; }
        public string VenueName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
