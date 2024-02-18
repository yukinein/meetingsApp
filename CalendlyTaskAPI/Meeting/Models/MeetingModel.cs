namespace CalendlyTaskAPI.Meeting.Models
{
    public class MeetingModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? InitiatorUserId { get; set; }
        public string? InitiatorFullName { get; set; }
    }
}
