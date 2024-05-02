using CalendlyTaskAPI.Core.DTOs.Responses;
using CalendlyTaskAPI.Core.Entities;
using CalendlyTaskAPI.Meeting.Requests;
using CalendlyTaskAPI.Meeting.Responses;

namespace CalendlyTaskAPI.Meeting.Interfaces
{
    public interface INotificationService
    {
        Task<List<UserNotification>> GetUserNotifications(string userId);
        Task<UserNotification> GetNotificationById(int notificationId);
        Task<OperationStatusResponse> DeleteRejectedNotification(string userId, int notificationId);

    }
}
