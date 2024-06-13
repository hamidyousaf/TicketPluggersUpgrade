namespace TP.Upgrade.Application.DTOs
{
    public class GetFavouriteResultDto
    {
        public IList<GetFavouriteEventDto> FavouriteEvents { get; set; }
        public IList<GetFavouritePerformerDto> FavouritePerformers { get; set; }
        public IList<GetFavouriteVenueDto> FavouriteVenues { get; set; }
    }
}
