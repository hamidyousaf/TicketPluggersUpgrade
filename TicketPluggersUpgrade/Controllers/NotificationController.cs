using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP.Upgrade.Api.Extensions;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Api.Controllers
{
    public class NotificationController : ApiBaseController
    {
        private readonly INotificationService _notificationServices;
        public NotificationController(INotificationService notificationServices)
        {
            _notificationServices = notificationServices;
        }
        [HttpGet("get-notifications")]
        public async Task<IActionResult> GetNotifications(string type = "All", int skip = 0, CancellationToken ct = default)
        {
            var result = await _notificationServices.GetNotifications(User.GetCustomerId(), type, skip, ct);
            return Ok(result);
        }
        [HttpPost("MarkAsReadNotification/{notificationId}")]
        public async Task<IActionResult> MarkAsReadNotification(Guid notificationId)
        {
            var result = await _notificationServices.MarkAsReadNotification(notificationId);
            return Ok(result);
        }
        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsReadNotifications()
        {
            var result = await _notificationServices.MarkAllAsReadNotifications(User.GetCustomerId());
            return Ok(result);
        }
    }
}
//Your ticket(s) has been uploaded on your order # 231. You can download your ticket(s)