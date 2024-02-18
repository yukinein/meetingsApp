using CalendlyTaskAPI.Core.DTOs.Responses;
using CalendlyTaskAPI.Meeting.Requests;
using CalendlyTaskAPI.Meeting.Responses;

namespace CalendlyTaskAPI.Meeting.Interfaces
{
    public interface IMeetingService
    {
        Task<GetMeetingsForUserResponse> GetMeetingsForUser(GetMeetingsForUserRequest request);
        Task<OperationStatusResponse> CreateMeeting(CreateMeetingRequest request);
        Task<DeleteMeetingResponse> DeleteMeeting(DeleteMeetingRequest request);
        Task<DropDownResponse> GetUsers(GetUsersRequest request);
    }
}
