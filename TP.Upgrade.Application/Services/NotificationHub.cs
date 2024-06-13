using Microsoft.AspNetCore.SignalR;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.Utils;

namespace TP.Upgrade.Application.Services
{
    public class NotificationHub : Hub
    {
        private readonly PresenceTracker _tracker;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;
        public NotificationHub(PresenceTracker tracker,
            ICustomerService customerService,
            INotificationService notificationService)
        {
            _tracker = tracker;
            _customerService = customerService;
            _notificationService = notificationService;
        }
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var userId = (long)Convert.ToInt32(http.Request.Query["userId"]);
            var customer = await _customerService.GetById(userId);
            if (customer != null)
            {
                await _tracker.UserConnected(customer.Username, Context.ConnectionId);
            }
        }
        public async Task GetTotalUnreadNotification()
        {
            var http = Context.GetHttpContext();
            var userId = (long)Convert.ToInt32(http.Request.Query["userId"]);
            var customer = await _customerService.GetById(userId);
            var connections = await PresenceTracker.GetConnectionsForUser(customer.Username);
            var notifications = await _notificationService.GetTotalUnreadNotification(userId);
            if (connections != null && notifications > 0)
            {
                await Clients.Clients(connections).SendAsync("GetTotalUnreadNotification", new { response = true, totalUnreadNotification = notifications });
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = (long)Context.Items["User"];
            var customer = await _customerService.GetById(userId);
            if (customer != null)
            {
                await _tracker.UserDisconnected(customer.Username, Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
