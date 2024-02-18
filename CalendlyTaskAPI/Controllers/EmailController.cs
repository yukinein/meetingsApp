using CalendlyTaskAPI.Notification.Interfaces;
using CalendlyTaskAPI.Notification.Requests;
using CalendlyTaskAPI.Notification.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CalendlyTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _service;

        public EmailController(IEmailService service)
        {
            _service = service;
        }

        [HttpPost("SendEmail")]
        [Authorize]
        public async Task<ActionResult<SendEmailResponse>> SendEmail(SendEmailRequest request)
        {
            return await _service.SendEmail(request);
        }
    }
}
