using Azure.Identity;
using CalendlyTaskAPI.Core.DbContext;
using CalendlyTaskAPI.Core.DTOs.Models;
using CalendlyTaskAPI.Core.DTOs.Responses;
using CalendlyTaskAPI.Core.Entities;
using CalendlyTaskAPI.Meeting.Interfaces;
using CalendlyTaskAPI.Meeting.Models;
using CalendlyTaskAPI.Meeting.Requests;
using CalendlyTaskAPI.Meeting.Responses;
using CalendlyTaskAPI.Notification.Interfaces;
using CalendlyTaskAPI.Notification.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CalendlyTaskAPI.Meeting.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public MeetingService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<GetMeetingsForUserResponse> GetMeetingsForUser(GetMeetingsForUserRequest request)
        {
            var meetings = await _context.Meeting
                .Where(x => x.UserId == request.Id)
                .OrderBy(x => x.StartDateTime)
                .AsNoTracking()
                .Select(x => new MeetingModel
                {
                    Id = x.Id,
                    GroupId = x.GroupId,
                    Name = x.UserName,
                    DurationInMinutes = x.DurationInMinutes,
                    StartDateTime = x.StartDateTime,
                    EndDateTime = x.EndDateTime,
                    Reason = x.Reason,
                    InitiatorUserId = x.InitiatorUserId,
                    InitiatorFullName = x.InitiatorFullName,
                    UserId = x.UserId,
                }).ToListAsync();

            foreach (var meeting in meetings)
            {
                if (meeting.InitiatorUserId != null)
                {
                    var initiator = await _userManager.FindByIdAsync(meeting.InitiatorUserId);
                    meeting.InitiatorFullName = initiator?.FullName;
                }
            }
            return new GetMeetingsForUserResponse { List = meetings };
        }

        public async Task<OperationStatusResponse> CreateMeeting(CreateMeetingRequest request, string initiatorUserId)
        {
            var user = await _userManager.FindByIdAsync(initiatorUserId);
            var recieverUser = await _userManager.FindByIdAsync(request.UserId);
            string userName = user.UserName;
            string recieverUserName = recieverUser.UserName;

            if (string.IsNullOrWhiteSpace(userName))
                return new OperationStatusResponse { IsSuccessful = false, Message = "Invalid name" };

            if (request.DurationInMinutes < 5)
                return new OperationStatusResponse { IsSuccessful = false, Message = "Meeting must last at least for 5 minutes." };

            if (request.StartDateTime < DateTimeOffset.Now)
                return new OperationStatusResponse { IsSuccessful = false, Message = "You cannot schedule a meeting in the past." };

            // Prepare the message to send as a notification
            string notificationMessage = $"You are invited by '{userName}' to the meeting planned for {request.StartDateTime}, " +
                                         $"which will last for {request.DurationInMinutes} minutes. Reason for the meeting: '{request.Reason}', " +
                                         $"Please approve or reject this meeting.";

            // Send a notification for meeting approval
            await SendNotificationAsync(request.UserId, recieverUserName, initiatorUserId, userName, notificationMessage, request.StartDateTime, request.DurationInMinutes, request.Reason);

            return new OperationStatusResponse { IsSuccessful = true, Message = "Meeting creation request has been sent." };
        }


        public async Task<DeleteMeetingResponse> DeleteMeeting(DeleteMeetingRequest request, string userId)
        {
            var meeting = await _context.Meeting.FirstOrDefaultAsync(x => x.Id == request.MeetingId);

            if (meeting == null)
            {
                return new DeleteMeetingResponse
                {
                    OperationStatusResponse = new OperationStatusResponse { IsSuccessful = false, Message = "Meeting does not exist." },
                };
            }

            if (meeting.UserId != userId)
            {
                return new DeleteMeetingResponse
                {
                    OperationStatusResponse = new OperationStatusResponse { IsSuccessful = false, Message = "Unauthorized: You can only delete meetings you have created." },
                };
            }

            // Find all meetings with the same GroupId to notify all affected users
            var relatedMeetings = await _context.Meeting.Where(x => x.GroupId == meeting.GroupId).ToListAsync();
            ApplicationUser initiator = await _userManager.FindByIdAsync(meeting.InitiatorUserId);
            ApplicationUser cancellingUser = await _userManager.FindByIdAsync(userId);

            foreach (var relMeeting in relatedMeetings)
            {
                ApplicationUser relatedUser = await _userManager.FindByIdAsync(relMeeting.UserId);
                string participantMessage = $"Your meeting scheduled on {relMeeting.StartDateTime} for '{relMeeting.Reason}' has been cancelled by {cancellingUser.FullName}.";
                string ownerMessage = $"Your meeting scheduled on {relMeeting.StartDateTime} with {initiator.FullName} for '{relMeeting.Reason}' has been cancelled.";

                // Determine the correct cancellation message based on the user
                string cancellationMessage = relMeeting.UserId == userId ? ownerMessage : participantMessage;

                // Send cancellation notification to each participant
                await SendNotificationAsync(relMeeting.UserId, relatedUser.UserName, userId, "System", cancellationMessage, DateTime.UtcNow, 0, "Meeting Cancelled");
                _context.Meeting.Remove(relMeeting);
            }

            await _context.SaveChangesAsync();

            return new DeleteMeetingResponse
            {
                OperationStatusResponse = new OperationStatusResponse { IsSuccessful = true, Message = $"All meetings with GroupId {meeting.GroupId} have been cancelled successfully." }
            };
        }




        public async Task<DropDownResponse> GetUsers(GetUsersRequest request)
        {
            DropDownResponse response = new();
            if (request.Id is null)
            {
                response.List = await _userManager.Users
                    .OrderBy(x => x.UserName)
                    .AsNoTracking()
                    .Select(x => new DropDownItem
                    {
                        Id = x.Id,
                        Name = x.UserName!
                    }).ToListAsync();
            }
            else
            {
                response.List = await _userManager.Users
                    .Where(x => x.Id !=  request.Id)
                    .OrderBy(x => x.FullName)
                    .AsNoTracking()
                    .Select(x => new DropDownItem
                    {
                        Id = x.Id,
                        Name = x.FullName
                    }).ToListAsync();
            }

            return response;
        }
        public async Task SendNotificationAsync(string userId, string userName, string initiatorUserId, string name, string message, DateTimeOffset startDateTime, int durationInMinutes, string reason, int? groupId = null)
        {
            var notification = new UserNotification
            {
                UserId = userId,
                UserName = userName,
                InitiatorUserId = initiatorUserId,
                InitiatorFullName = name,
                Message = message,
                Reason = reason,
                DurationInMinutes = durationInMinutes,
                StartDateTime = startDateTime.DateTime,
                EndDateTime = startDateTime.DateTime.AddMinutes(durationInMinutes),
                IsRead = false,
                GroupId = groupId
            };

            _context.UserNotifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsMeetingSlotAvailable(string userId, DateTimeOffset startDateTime, int durationInMinutes)
        {
            var endDateTime = startDateTime.AddMinutes(durationInMinutes);
            return !(await _context.Meeting.AnyAsync(m => m.UserId == userId &&
                                                         m.StartDateTime < endDateTime &&
                                                         m.EndDateTime > startDateTime));
        }

        public async Task<OperationStatusResponse> ApproveMeeting(int notificationId, string approverUserId)
        {
            var notification = await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == approverUserId);
            bool isMeetingSlotAvailable = await IsMeetingSlotAvailable(approverUserId, notification.StartDateTime, notification.DurationInMinutes);
            if (!isMeetingSlotAvailable)
            {
                return new OperationStatusResponse { IsSuccessful = false, Message = "Time slot not available." };
            }

            if (notification == null || notification.IsRead)
            {
                return new OperationStatusResponse { IsSuccessful = false, Message = "Notification not found or already processed." };
            }


            // Rest of your existing logic to create the meeting

            if (notification.GroupId != null)
            {
                // Delete all meetings with the same GroupId
                IEnumerable<Core.Entities.Meeting> meetingsToDelete = await _context.Meeting.Where(m => m.GroupId == notification.GroupId).ToListAsync();
                _context.Meeting.RemoveRange(meetingsToDelete);
                await _context.SaveChangesAsync();
            }

            int groupId = GenerateGroupId();

            Core.Entities.Meeting meeting = new Core.Entities.Meeting
            {
                GroupId = groupId,
                DurationInMinutes = notification.DurationInMinutes,
                StartDateTime = notification.StartDateTime,
                EndDateTime = notification.StartDateTime.AddMinutes(notification.DurationInMinutes),
                Reason = notification.Reason,
                UserId = notification.UserId,
                UserName = notification.UserName,
                InitiatorUserId = notification.InitiatorUserId,
                InitiatorFullName = notification.InitiatorFullName
            };

            // Create a new meeting for the InitiatorUser
            Core.Entities.Meeting initiatorMeeting = new Core.Entities.Meeting
            {
                GroupId = groupId,
                DurationInMinutes = notification.DurationInMinutes,
                StartDateTime = notification.StartDateTime,
                EndDateTime = notification.StartDateTime.AddMinutes(notification.DurationInMinutes),
                Reason = notification.Reason,
                UserId = notification.InitiatorUserId,
                UserName = notification.InitiatorFullName,
                InitiatorUserId = notification.UserId,
                InitiatorFullName = notification.UserName
            };

            _context.Meeting.Add(meeting);
            _context.Meeting.Add(initiatorMeeting);

            await _context.SaveChangesAsync();

            // Mark notification as read
            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return new OperationStatusResponse { IsSuccessful = true, Message = "Meeting has been created successfully." };
        }

        private int GenerateGroupId()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

public async Task<UpdateMeetingResponse> UpdateMeeting(UpdateMeetingRequest request, string userId)
{
    var meeting = await _context.Meeting.Where(x => x.Id == request.MeetingId).FirstOrDefaultAsync();

    if (meeting == null)
    {
        return new UpdateMeetingResponse { IsSuccessful = false, Message = "Meeting does not exist" };
    }

    if (meeting.UserId != userId) // Ensure that only the owner can update their meeting
    {
        return new UpdateMeetingResponse { IsSuccessful = false, Message = "Unauthorized access" };
    }


    // Prepare the message to send as a notification
    string notificationMessage = $"The meeting with ID {request.MeetingId} has been updated. " +
                                  $"New start time: {request.NewStartDateTime}, " +
                                  $"New end time: {request.NewStartDateTime.AddMinutes(request.NewDurationInMinutes)}, " +
                                  $"New duration: {request.NewDurationInMinutes} minutes, " +
                                  $"New reason: {request.NewReason}. " +
                                  $"Please approve or reject this update.";

    // Send a notification for meeting approval
    await SendNotificationAsync(meeting.UserId, meeting.UserName, meeting.InitiatorUserId, meeting.InitiatorFullName, notificationMessage, request.NewStartDateTime, request.NewDurationInMinutes, request.NewReason, meeting.GroupId);

    return new UpdateMeetingResponse { IsSuccessful = true, Message = "Meeting update request has been sent." };
}
    }
}
