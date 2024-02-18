using System.ComponentModel.DataAnnotations;

namespace CalendlyTaskAPI.Core.DTOs.Requests
{
    public class UpdatePermissionRequest
    {
        public string Id { get; set; } = string.Empty;
    }
}
