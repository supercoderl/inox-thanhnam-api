using InoxThanhNamServer.Datas.Notification;
using InoxThanhNamServer.Services.NotificationSer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
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

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var result = await _notificationService.GetNotifications();
            return StatusCode(result.Status, result);
        }

        [HttpPut("update-notification/{NotificationID}")]
        public async Task<IActionResult> UpdateNotification(int NotificationID, UpdateNotificationRequest request)
        {
            var result = await _notificationService.UpdateNotification(NotificationID, request);
            return StatusCode(result.Status, result);
        }
    }
}
