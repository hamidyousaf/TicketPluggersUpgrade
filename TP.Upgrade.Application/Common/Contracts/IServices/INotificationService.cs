using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Response;

namespace TP.Upgrade.Application.Common.Contracts.IServices
{
    public interface INotificationService
    {
        Task<List<UnreadNotificationDto>> GetNotifications(long userId, string type = "All", int skip = 0, CancellationToken ct = default);
        Task<bool> MarkAsReadNotification(Guid notificationId);
        Task<int> GetTotalUnreadNotification(long userId);
        Task<bool> MarkAllAsReadNotifications(long userId);
        Task SendNotificationToVendorOnBuy(NotificationDto notificationParams);
        Task SendNotificationOnOrderConfirmation(NotificationDto notificationParams);
        Task SendNotificationToAdminOnPayRequest(NotificationDto notificationParams);
        Task SendNotificationToVendorOnPayRequestApproval(NotificationDto notificationParams);
    }
}
