namespace CalendlyTaskAPI.Core.DTOs.Requests
{
    public class RegisterRequestModel
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IFormFile? Picture { get; set; }
    }
}
