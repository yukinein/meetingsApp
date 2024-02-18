using CalendlyTaskAPI.Core.DTOs.Requests;
using CalendlyTaskAPI.Core.DTOs.Responses;
using CalendlyTaskAPI.Meeting.Interfaces;
using CalendlyTaskAPI.Meeting.Requests;
using CalendlyTaskAPI.Meeting.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CalendlyTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _service;

        public MeetingController(IMeetingService service)
        {
            _service = service;
        }

        [HttpPost("GetMeetingsForUser")]
        public async Task<ActionResult<GetMeetingsForUserResponse>> GetMeetingsForUser(GetMeetingsForUserRequest request)
        {
            return await _service.GetMeetingsForUser(request);
        }

        [HttpPost("CreateMeeting")]
        public async Task<ActionResult<OperationStatusResponse>> CreateMeeting(CreateMeetingRequest request)
        {
            return await _service.CreateMeeting(request);
        }

        [HttpPost("DeleteMeeting")]
        public async Task<ActionResult<DeleteMeetingResponse>> DeleteMeeting(DeleteMeetingRequest request)
        {
            return await _service.DeleteMeeting(request);
        }

        [HttpPost("GetUsers")]
        public async Task<ActionResult<DropDownResponse>> GetUsers(GetUsersRequest request)
        {
            return await _service.GetUsers(request);
        }
    }
}
