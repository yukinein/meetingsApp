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
using Microsoft.EntityFrameworkCore;

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
                    Name = x.Name,
                    DurationInMinutes = x.DurationInMinutes,
                    StartDateTime = x.StartDateTime,
                    Reason = x.Reason,
                    InitiatorUserId = x.InitiatorUserId
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

        public async Task<OperationStatusResponse> CreateMeeting(CreateMeetingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new OperationStatusResponse { IsSuccessful = false, Message = "Invalid name" };

            if (request.DurationInMinutes < 5)
                return new OperationStatusResponse { IsSuccessful = false, Message = "Meeting must last atleast for 5 minutes." };

            if (request.StartDateTime < DateTimeOffset.Now)
                return new OperationStatusResponse { IsSuccessful = false, Message = "You can schedule meeting in the past." };

            if (await HasOverlap(request.StartDateTime, request.DurationInMinutes, request.UserId))
                return new OperationStatusResponse { IsSuccessful = false, Message = "There is overlap with other meetings" };

            Core.Entities.Meeting meeting = new()
            {
                Name = request.Name,
                DurationInMinutes = request.DurationInMinutes,
                StartDateTime = request.StartDateTime,
                EndDateTime = request.StartDateTime.AddMinutes(request.DurationInMinutes),
                Reason = request.Reason,
                UserId = request.UserId,
                InitiatorUserId = request.InitiatorUserId
            };

            _context.Add(meeting);

            int rowsChanged = await _context.SaveChangesAsync();

            if(request.InitiatorUserId is not null)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                var initiator = await _userManager.FindByIdAsync(request.InitiatorUserId);

                SendEmailRequest emailRequest = new SendEmailRequest
                {
                    EmailTo = new List<string> { user!.Email! },
                    EmailSubject = "You have new meeting.",
                    EmailBody = $"Hello, {initiator!.Email} scheduled a meeting with you starting at {request.StartDateTime}, with duration of {request.DurationInMinutes} minutes, reason: {request.Reason}"
                };

                await _emailService.SendEmail(emailRequest);
            }

            if (rowsChanged > 0)
                return new OperationStatusResponse { IsSuccessful = true, Message = "Meeting has been saved." };
            else
                return new OperationStatusResponse { IsSuccessful = false, Message = "Something went wrong" };
        }

        private async Task<bool> HasOverlap(DateTimeOffset startDateTime, int durationInMinutes, string userId)
        {
            DateTimeOffset endDateTime = startDateTime.AddMinutes(durationInMinutes);

            bool hasOverlap = await _context.Meeting.AnyAsync(x => x.StartDateTime < endDateTime && x.EndDateTime > startDateTime && x.UserId == userId);

            return hasOverlap;
        }

        public async Task<DeleteMeetingResponse> DeleteMeeting(DeleteMeetingRequest request)
        {
            var meeting = await _context.Meeting.Where(x => x.Id == request.MeetingId).FirstOrDefaultAsync();

            if (meeting is null)
            {
                return new DeleteMeetingResponse
                {
                    OperationStatusResponse = new OperationStatusResponse { IsSuccessful = false, Message = "Meeting does not exist" },
                    GetMeetingsForUserResponse = await GetMeetingsForUser(request.GetMeetingsForUserRequest)
                };
            }

            _context.Remove(meeting);

            int rowsChanged = await _context.SaveChangesAsync();

            if (rowsChanged > 0)
            {
                return new DeleteMeetingResponse
                {
                    OperationStatusResponse = new OperationStatusResponse { IsSuccessful = true, Message = $"Success. Meeting with ID {meeting.Id} deleted successfully." },
                    GetMeetingsForUserResponse = await GetMeetingsForUser(request.GetMeetingsForUserRequest)
                };
            }
            else
            {
                return new DeleteMeetingResponse
                {
                    OperationStatusResponse = new OperationStatusResponse { IsSuccessful = false, Message = "Something went wrong" },
                    GetMeetingsForUserResponse = await GetMeetingsForUser(request.GetMeetingsForUserRequest)
                };
            }
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
    }
}
