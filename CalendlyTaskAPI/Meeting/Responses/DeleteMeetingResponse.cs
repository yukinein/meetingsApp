using CalendlyTaskAPI.Core.DTOs.Responses;

namespace CalendlyTaskAPI.Meeting.Responses
{
    public class DeleteMeetingResponse
    {
        public OperationStatusResponse OperationStatusResponse { get; set; } = new();
    }
}
