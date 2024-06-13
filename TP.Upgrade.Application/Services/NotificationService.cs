using Humanizer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Application.Utils;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ICustomerService _customerService;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository, ICustomerService customerService)
        {
            _notificationRepository = notificationRepository;
            _customerService = customerService;
        }
        public async Task<List<UnreadNotificationDto>> GetNotifications(long userId, string type = "All", int skip = 0, CancellationToken ct = default)
        {
            List<UnreadNotificationDto> result = new List<UnreadNotificationDto>();
            IQueryable<UnreadNotificationDto> query = _notificationRepository
                .GetReadOnlyList()
                .Where(x => x.ToUserId == userId)
                .OrderByDescending(x => x.NotificationDate)
                .Select(x => new UnreadNotificationDto
                {
                    NotificationId = x.NotificationId,
                    NotificationDate = x.NotificationDate,
                    NotificationContent = x.NotificationContent,
                    IsRead = x.IsRead,
                    NotificationEndpoint = x.NotificationEndpoint
                });
            if (type == "Unread")
            {
                query = query.Where(x => !x.IsRead);
            }
            var notifications = await query.Skip(skip).Take(20).ToListAsync(ct);
            foreach (var notification in notifications)
            {
                DateTime dtNow = DateTime.UtcNow;
                TimeSpan results = dtNow.Subtract(notification.NotificationDate);
                int seconds = Convert.ToInt32(results.TotalSeconds);
                notification.TimeAgo = DateTime.Now.AddSeconds(-seconds).Humanize();
                result.Add(notification);
            }
            return result;
        }
        public async Task<bool> MarkAsReadNotification(Guid notificationId)
        {
            var notification = await _notificationRepository.GetAll().FirstOrDefaultAsync(x => x.NotificationId == notificationId);
            if (notification is null) return false;
            notification.IsRead = true;
            await _notificationRepository.Change(notification);
            await SendTotalUnreadNotificationByUserId(notification.ToUserId);
            return true;
        }
        private async Task SendTotalUnreadNotificationByUserId(long userId)
        {
            var totalNotifications = await GetTotalUnreadNotification(userId);
            var username = _customerService.GetById(userId).Result.Username;
            var connections = await PresenceTracker.GetConnectionsForUser(username);
            if (connections != null)
            {
                await _notificationHub.Clients.Clients(connections)
                    .SendAsync("GetTotalUnreadNotification", new { response = true, totalUnreadNotification = totalNotifications });
            }
        }
        public async Task<int> GetTotalUnreadNotification(long userId)
        {
            var total = await _notificationRepository.GetAll().Where(x => !x.IsRead && x.ToUserId == userId).CountAsync();
            return total;
        }
        public async Task<bool> MarkAllAsReadNotifications(long userId)
        {
            var notifications = await _notificationRepository.GetAll().Where(x => x.ToUserId == userId && !x.IsRead).ToListAsync();
            if (notifications.Count > 0)
            {
                notifications.ForEach(x => x.IsRead = true);
                await _notificationRepository.ChangeRange(notifications);
                await SendTotalUnreadNotificationByUserId(userId);
            }
            return true;
        }
        public async Task SendNotificationToVendorOnBuy(NotificationDto notificationParams)
        {
            await AddNotificationToVendorOnBuy(notificationParams);  // Add Notification
            await SendNotification(notificationParams, "NewOrderPlaced");  // Send Notification
        }
        private async Task<bool> AddNotificationToVendorOnBuy(NotificationDto notificationParams)
        {
            var notification = new Notification()
            {
                OrderId = notificationParams.OrderId,
                NotificationContent = $"A new order # {notificationParams.OrderId} has been placed.",
                FromUserId = notificationParams.FromUserId,
                ToUserId = notificationParams.ToUserId,
                NotificationEndpoint = notificationParams.ToUserId == 3 ? "/admin/sales" : "/user/sales" // here UserId of admin is 3 so path of admin different than user.
            };
            await _notificationRepository.Add(notification);
            return true;
        }
        private async Task SendNotification(NotificationDto notificationParams, string notificationType)
        {
            var connections = await PresenceTracker.GetConnectionsForUser(notificationParams.ToUsername);
            if (connections != null)
            {
                await _notificationHub.Clients.Clients(connections).SendAsync(notificationType, "");
                await SendTotalUnreadNotificationByUserId(notificationParams.ToUserId);
            }
        }
        public async Task SendNotificationOnOrderConfirmation(NotificationDto notificationParams)
        {
            await AddNotificationOnOrderConfirmation(notificationParams);  // Add Notification
            await SendNotification(notificationParams, "OrderConfirmation");  // Send Notification
        }
        private async Task<bool> AddNotificationOnOrderConfirmation(NotificationDto notificationParams)
        {
            var notification = new Notification()
            {
                OrderId = notificationParams.OrderId,
                NotificationContent = $"The seller has confirmed your order # {notificationParams.OrderId}. You will receive your ticket(s) very soon.",
                FromUserId = notificationParams.FromUserId,
                ToUserId = notificationParams.ToUserId,
                NotificationEndpoint = "/user/orders"
            };
            await _notificationRepository.Add(notification);
            return true;
        }
        public async Task SendNotificationToAdminOnPayRequest(NotificationDto notificationParams)
        {
            await AddNotificationOnPayRequest(notificationParams);  // Add Notification
            await SendNotification(notificationParams, "PayRequest");  // Send Notification
        }
        private async Task<bool> AddNotificationOnPayRequest(NotificationDto notificationParams)
        {
            var notification = new Notification()
            {
                OrderId = notificationParams.OrderId,
                NotificationContent = $"{notificationParams.FromUsername} has requested payment for order # {notificationParams.OrderId}.",
                FromUserId = notificationParams.FromUserId,
                ToUserId = notificationParams.ToUserId,
                NotificationEndpoint = "/admin/payments"
            };
            await _notificationRepository.Add(notification);
            return true;
        }
        public async Task SendNotificationToVendorOnPayRequestApproval(NotificationDto notificationParams)
        {
            await AddNotificationOnPayRequestApproval(notificationParams);  // Add Notification
            await SendNotification(notificationParams, "PayRequestApproval");  // Send Notification
        }
        private async Task<bool> AddNotificationOnPayRequestApproval(NotificationDto notificationParams)
        {
            var notification = new Notification()
            {
                OrderId = notificationParams.OrderId,
                NotificationContent = $"Your order # {notificationParams.OrderId} has been approved.",
                FromUserId = notificationParams.FromUserId,
                ToUserId = notificationParams.ToUserId,
                NotificationEndpoint = "/user/payments"
            };
            await _notificationRepository.Add(notification);
            return true;
        }
    }
}
