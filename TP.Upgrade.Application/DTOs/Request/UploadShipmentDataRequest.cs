namespace TP.Upgrade.Application.DTOs.Request
{
    public class UploadShipmentDataRequest
    {
        public long OrderId { get; set; }
        public long CourierAgencyId { get; set; }
        public string TrackId { get; set; }
    }
}
