namespace CalendlyTaskAPI.Meeting.Requests
{
    public class CreateMeetingRequest
    {
        public string Name { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? InitiatorUserId { get; set; }
    }
}
