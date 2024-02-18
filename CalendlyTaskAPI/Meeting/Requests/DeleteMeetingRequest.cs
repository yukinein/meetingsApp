namespace CalendlyTaskAPI.Meeting.Requests
{
    public class DeleteMeetingRequest
    {
        public int MeetingId { get; set; }
        public GetMeetingsForUserRequest GetMeetingsForUserRequest { get; set; } = new();
    }
}
