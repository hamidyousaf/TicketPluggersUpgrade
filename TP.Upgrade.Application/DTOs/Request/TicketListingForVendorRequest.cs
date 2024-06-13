namespace TP.Upgrade.Application.DTOs.Request
{
    public class TicketListingForVendorRequest
    {
        public long customerId { get; set; }
        public int Tab { get; set; }
        public string searchText { get; set; }
        public int start { get; set; }
        public int rows { get; set; }
    }
}
