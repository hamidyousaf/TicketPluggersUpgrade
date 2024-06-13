namespace TP.Upgrade.Application.DTOs.Request
{
    public class TicketMarketAnalysisRequest
    {
        public long eventId { get; set; }
        public long customerId { get; set; }
        public bool myListing { get; set; }
        public string filterBy { get; set; }   //section,venue,quantity
        public string type { get; set; }
    }
}
