namespace CalendlyTaskAPI.Core.DTOs.Responses
{
    public class OperationStatusResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
