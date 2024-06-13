namespace TP.Upgrade.Application.DTOs
{
    public class DownloadTicketDto
    {
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public long orderId { get; set; }
        public long documentId { get; set; }
        public bool IsDownloaded { get; set; }
    }
}
