namespace CalendlyTaskAPI.Core.DTOs.Responses
{
    public class AuthServiceResponse
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; } = string.Empty;
        public byte[]? Picture { get; set; } = null;
    }
}
