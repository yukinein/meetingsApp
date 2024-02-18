namespace CalendlyTaskAPI.Core.Entities
{
    public class Meeting
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? InitiatorUserId { get; set; }
    }
}
