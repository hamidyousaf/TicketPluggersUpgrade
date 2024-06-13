namespace TP.Upgrade.Application.DTOs.Request
{
    public class ApproveShipmentDateRequest
    {
        public List<long> OrderIds { get; set; }
        public bool Status { get; set; }
    }
}
