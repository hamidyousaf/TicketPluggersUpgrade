namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Notification : BaseEntity<long>
    {
        public Notification()
        {
            NotificationId = Guid.NewGuid();
        }
        public Guid NotificationId { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationContent { get; set; }
        public string NotificationEndpoint { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public string Subject { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRead { get; set; }
        public long OrderId { get; set; }
    }
}
