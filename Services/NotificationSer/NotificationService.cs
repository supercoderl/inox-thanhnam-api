using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Notification;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InoxThanhNamServer.Services.NotificationSer
{
    public class NotificationService : INotificationService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;

        public NotificationService(InoxEcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ApiResponse<List<NotificationProfile>>> GetNotifications()
        {
            try
            {
                await Task.CompletedTask;
                var notifications = await _context.Notifications.OrderByDescending(x => x.CreatedAt).ToListAsync();
                if(!notifications.Any())
                {
                    return new ApiResponse<List<NotificationProfile>>
                    {
                        Success = false,
                        Message = "Không có thông báo.",
                        Status = (int)HttpStatusCode.OK
                    };
                }
                return new ApiResponse<List<NotificationProfile>>
                {
                    Success = true,
                    Message = "Tìm thấy thông báo",
                    Data = notifications.Select(x => _mapper.Map<NotificationProfile>(x)).ToList(),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<NotificationProfile>>
                {
                    Success = false,
                    Message = "NotificationService - GetNotifications: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<NotificationProfile>> UpdateNotification(int NotificationID, UpdateNotificationRequest request)
        {
            try
            {
                await Task.CompletedTask;

                if (NotificationID != request.NotificationID)
                {
                    return new ApiResponse<NotificationProfile>
                    {
                        Success = false,
                        Message = "Thông báo không đúng.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                var notificationEntity = await _context.Notifications.FindAsync(NotificationID);

                if (notificationEntity == null)
                {
                    return new ApiResponse<NotificationProfile>
                    {
                        Success = false,
                        Message = "Không thể cập nhật vì thông báo không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                _mapper.Map(request, notificationEntity);
                _context.Notifications.Update(notificationEntity);
                await _context.SaveChangesAsync();

                return new ApiResponse<NotificationProfile>
                {
                    Success = true,
                    Message = "Cập nhật thông báo thành công.",
                    Data = _mapper.Map<NotificationProfile>(notificationEntity),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<NotificationProfile>
                {
                    Success = false,
                    Message = "NotificationService - UpdateNotification: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
