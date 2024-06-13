namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetListingPriceWithCheckRequest
    {
        public int eventId { get; set; }
        public string currency { get; set; }
        public int ticketId { get; set; }
        public int quantity_selected { get; set; }
    }
}
