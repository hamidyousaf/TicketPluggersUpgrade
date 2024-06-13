namespace TP.Upgrade.Application.DTOs.Request
{
    public class UpdatePublishStatusRequest
    {
        public long Id { get; set; }
        public bool Publish { get; set; }
    }
}
