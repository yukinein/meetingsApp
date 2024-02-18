using Microsoft.AspNetCore.Identity;

namespace CalendlyTaskAPI.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public byte[]? Picture { get; set; }
    }
}
