using Li_copy.I_InterfaceLayer.NotificationInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Li_copy.ControllersLayer.Notification
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("Admin")]
        public async Task<IActionResult> GetAdminNotifications()
        {
            var result = await _notificationService.GetAdminNotificationsAsync();
            return Ok(result);
        }

        [HttpGet("librarian")]
        public async Task<IActionResult> GetLibrarianNotification()
        {
            // Handed off directly to the service layer method
            var result = await _notificationService.GetLibrarianNotificationsAsync();
            return Ok(result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);
            return result ? Ok(new { message = "Marked as read" }) : NotFound();
        }

        [HttpGet("unread-admin")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _notificationService.GetAdminUnreadCountAsync();
            return Ok(count);
        }

        [HttpGet("unread-librarian")]
        public async Task<IActionResult> GetLibrarianCount()
        {
            var count = await _notificationService.GetLibrarianUnreadCountAsync();
            return Ok(count);
        }
    }
}