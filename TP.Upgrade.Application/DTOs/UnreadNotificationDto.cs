using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Upgrade.Application.DTOs
{
    public class UnreadNotificationDto
    {
        public Guid NotificationId { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationContent { get; set; }
        public string NotificationEndpoint { get; set; }
        public bool IsRead { get; set; }
        public string TimeAgo { get; set; }
    }
}
