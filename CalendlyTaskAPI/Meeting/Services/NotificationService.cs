using CalendlyTaskAPI.Core.Entities;
using CalendlyTaskAPI.Core.DbContext;
using Microsoft.EntityFrameworkCore;
using CalendlyTaskAPI.Meeting.Interfaces;
using CalendlyTaskAPI.Core.DTOs.Responses;

namespace CalendlyTaskAPI.Meeting.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the DbContext
        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<UserNotification> GetNotificationById(int notificationId)
        {
            return await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == notificationId);
        }
        public async Task<List<UserNotification>> GetUserNotifications(string userId)
        {
            return await _context.UserNotifications
                                 .Where(n => n.UserId == userId && !n.IsRead)
                                 .OrderByDescending(n => n.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<OperationStatusResponse> DeleteRejectedNotification(string userId, int notificationId)
        {
            var notification = await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == notificationId);
            if (notification == null)
            {
                return new OperationStatusResponse { IsSuccessful = false, Message = "Notification not found" };
            }

            // Check if the notification belongs to the user attempting to delete it
            if (notification.UserId != userId)
            {
                return new OperationStatusResponse { IsSuccessful = false, Message = "Unauthorized access" };
            }

            _context.UserNotifications.Remove(notification);
            await _context.SaveChangesAsync();

            return new OperationStatusResponse { IsSuccessful = true, Message = "Notification deleted successfully." };
        }
    }
}
