namespace TP.Upgrade.Application.DTOs
{
    public class NotificationDto
    {
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public string FromUsername { get; set; }
        public string ToUsername { get; set; }
        public long OrderId { get; set; }
    }
}
