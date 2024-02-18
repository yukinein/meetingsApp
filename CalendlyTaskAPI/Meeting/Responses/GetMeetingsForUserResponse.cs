using CalendlyTaskAPI.Meeting.Models;

namespace CalendlyTaskAPI.Meeting.Responses
{
    public class GetMeetingsForUserResponse
    {
        public List<MeetingModel> List { get; set; } = new();
    }
}
