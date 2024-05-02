namespace CalendlyTaskAPI.Core.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string InitiatorFullName { get; set; }
        public string InitiatorUserId { get; set; }
        public int DurationInMinutes {  get; set; }
        public DateTime StartDateTime {  get; set; } = DateTime.UtcNow;
        public DateTime EndDateTime { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; }
        public int? GroupId { get; internal set; }
    }
}
