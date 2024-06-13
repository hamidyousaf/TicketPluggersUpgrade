namespace TP.Upgrade.Application.DTOs.Request
{
    public class ResetShipmentRequest
    {
        public long OrderId { get; set; }
        public string Reason { get; set; }
    }
}
