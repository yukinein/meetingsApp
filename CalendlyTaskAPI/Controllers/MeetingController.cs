using CalendlyTaskAPI.Core.DTOs.Requests;
using CalendlyTaskAPI.Core.DTOs.Responses;
using CalendlyTaskAPI.Core.Entities;
using CalendlyTaskAPI.Meeting.Interfaces;
using CalendlyTaskAPI.Meeting.Requests;
using CalendlyTaskAPI.Meeting.Responses;
using CalendlyTaskAPI.Meeting.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CalendlyTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public MeetingController(IMeetingService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpPost("GetMeetingsForUser")]
        public async Task<ActionResult<GetMeetingsForUserResponse>> GetMeetingsForUser()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var request = new GetMeetingsForUserRequest { Id = userId };
            return await _service.GetMeetingsForUser(request);
        }

        [HttpPost("CreateMeeting")]
        public async Task<ActionResult<OperationStatusResponse>> CreateMeeting(CreateMeetingRequest request)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return await _service.CreateMeeting(request, userId);
        }

        [HttpPost("DeleteMeeting")]
        public async Task<ActionResult<DeleteMeetingResponse>> DeleteMeeting(DeleteMeetingRequest request)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return await _service.DeleteMeeting(request, userId);
        }

        [HttpPost("UpdateMeeting")]
        public async Task<ActionResult<UpdateMeetingResponse>> UpdateMeeting(UpdateMeetingRequest request)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return await _service.UpdateMeeting(request, userId);
        }

        [HttpPost("GetUsers")]
        public async Task<ActionResult<DropDownResponse>> GetUsers(GetUsersRequest request)
        {
            return await _service.GetUsers(request);
        }
    }
}
