using CalendlyTaskAPI.Notification.Requests;
using CalendlyTaskAPI.Notification.Responses;

namespace CalendlyTaskAPI.Notification.Interfaces
{
    public interface IEmailService
    {
        public Task<SendEmailResponse> SendEmail(SendEmailRequest request);
    }
}
