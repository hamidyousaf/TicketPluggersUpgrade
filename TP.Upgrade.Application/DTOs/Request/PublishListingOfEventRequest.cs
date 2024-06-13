namespace TP.Upgrade.Application.DTOs.Request
{
    public class PublishListingOfEventRequest
    {
        public long eventId { get; set; }
        public bool Published { get; set; }
    }
}
