namespace CalendlyTaskAPI.Meeting.Requests
{
    public class UpdateMeetingRequest
    {
        public int MeetingId { get; set; }
        public DateTimeOffset NewStartDateTime { get; set; }
        public string NewReason { get; set; }
        public int NewDurationInMinutes { get; set; }
        public string? InitiatorUserId { get; internal set; }
    }
}
