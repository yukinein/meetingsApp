using CalendlyTaskAPI.Core.DTOs.Models;

namespace CalendlyTaskAPI.Core.DTOs.Responses
{
    public class DropDownResponse
    {
        public List<DropDownItem> List { get; set; } = new();
    }
}
