namespace TP.Upgrade.Application.DTOs
{
    public class GetOrdersByTicketTypeIdDto
    {
        public long OrderId { get; set; }
        public DateTime? CurrentDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public string EventName { get; set; }
        public string SellorName { get; set; }
        public string? Reason { get; set; }
        public DateTime? ExpectShippingDate { get; set; }
        public DateTime? ResetShipmentRequestDate { get; set; }
    }
}
