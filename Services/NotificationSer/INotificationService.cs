using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Notification;

namespace InoxThanhNamServer.Services.NotificationSer
{
    public interface INotificationService
    {
        Task<ApiResponse<List<NotificationProfile>>> GetNotifications();

        Task<ApiResponse<NotificationProfile>> UpdateNotification(int NotificationID, UpdateNotificationRequest request);
    }
}
