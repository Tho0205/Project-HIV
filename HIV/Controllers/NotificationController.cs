
using HIV.DTOs;
using HIV.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            var notifications = await _notificationService.GetNotificationsForUser(userId);
            return Ok(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto dto)
        {
            var notification = await _notificationService.CreateNotification(dto);
            return CreatedAtAction(nameof(GetUserNotifications), new { userId = dto.UserId }, notification);
        }

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var result = await _notificationService.MarkAsRead(notificationId);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("user/{userId}/read-all")]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            var result = await _notificationService.MarkAllAsRead(userId);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var result = await _notificationService.DeleteNotification(notificationId);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredNotifications([FromQuery] NotificationFilterDto filter)
        {
            var notifications = await _notificationService.GetFilteredNotifications(filter);
            return Ok(notifications);
        }
    }
}