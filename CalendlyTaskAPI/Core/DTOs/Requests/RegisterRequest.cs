using System.ComponentModel.DataAnnotations;

namespace CalendlyTaskAPI.Core.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        public byte[]? Picture { get; set; }
    }
}
