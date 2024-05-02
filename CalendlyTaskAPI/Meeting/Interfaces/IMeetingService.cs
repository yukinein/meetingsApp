using CalendlyTaskAPI.Core.DTOs.Responses;
using CalendlyTaskAPI.Meeting.Requests;
using CalendlyTaskAPI.Meeting.Responses;

namespace CalendlyTaskAPI.Meeting.Interfaces
{
    public interface IMeetingService
    {
        Task<GetMeetingsForUserResponse> GetMeetingsForUser(GetMeetingsForUserRequest request);
        Task<OperationStatusResponse> CreateMeeting(CreateMeetingRequest request, string initiatorUserId);
        Task<DeleteMeetingResponse> DeleteMeeting(DeleteMeetingRequest request, string userId);
        Task<DropDownResponse> GetUsers(GetUsersRequest request);
        Task<OperationStatusResponse> ApproveMeeting(int notificationId, string userId);
        Task<UpdateMeetingResponse> UpdateMeeting(UpdateMeetingRequest request, string userId);
    }

}
