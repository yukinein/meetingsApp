using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendlyTaskAPI.Core.DbContext;
using CalendlyTaskAPI.Core.Entities;
using CalendlyTaskAPI.Meeting.Interfaces;
using CalendlyTaskAPI.Notification.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CalendlyTaskAPI.Core.DTOs.Responses;

namespace CalendlyTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Kullanıcı doğrulama gerektirir
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMeetingService _meetingService;

        public NotificationsController(INotificationService notificationService, IMeetingService meetingService)
        {
            _notificationService = notificationService;
            _meetingService = meetingService;
        }

        [HttpGet("GetNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User must be authenticated.");
            }

            List<UserNotification> notifications = await _notificationService.GetUserNotifications(userId);
            return Ok(notifications);
        }

        [HttpPost("approve/{notificationId}")]
        public async Task<IActionResult> ApproveNotification(int notificationId)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User must be authenticated.");
            }

            var notification = await _notificationService.GetNotificationById(notificationId);
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            if (notification.UserId != userId)
            {
                return BadRequest("Unauthorized to approve this notification.");
            }

            var response = await _meetingService.ApproveMeeting(notificationId, userId);
            if (response.IsSuccessful)
            {
                return Ok("Meeting approved successfully.");
            }
            else
            {
                // İşlem başarısız olduğunda, response nesnesinden gelen hata mesajını döndür
                return BadRequest(response.Message);
            }
        }


        [HttpDelete("DeleteNotification/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User must be authenticated.");
            }

            var notification = await _notificationService.GetNotificationById(notificationId);
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            if (notification.UserId != userId)
            {
                return BadRequest("Unauthorized to delete this notification.");
            }

            OperationStatusResponse response = await _notificationService.DeleteRejectedNotification(userId, notificationId);
            if (!response.IsSuccessful)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Message);
        }
    }
}
