namespace CalendlyTaskAPI.Meeting.Requests
{
    public class CreateMeetingRequest
    {
        public int DurationInMinutes { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public string Reason { get; set; }
        public string UserId { get; set; }
    }

}
