using Li_copy.I_InterfaceLayer.NotificationInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Li_copy.ControllersLayer.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationDLL _notificationDLL;

        public NotificationsController(INotificationDLL notificationDLL)
        {
            _notificationDLL = notificationDLL;
        }
        [HttpGet]
        public async Task<IActionResult> GetAdminNotifications()
        {
            var result = await _notificationDLL.GetAdminNotificationsAsync();
            return Ok(result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationDLL.MarkAsReadAsync(id);
            return result ? Ok("Marked as read") : NotFound();
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var all = await _notificationDLL.GetAdminNotificationsAsync();
            var count = all.Count(x => !x.IsRead);
            return Ok(count);
        }


    }
}
