using CalendlyTaskAPI.Core.Interfaces;
using CalendlyTaskAPI.Core.Services;
using CalendlyTaskAPI.Meeting.Interfaces;
using CalendlyTaskAPI.Meeting.Services;
using CalendlyTaskAPI.Notification.Interfaces;
using CalendlyTaskAPI.Notification.Services;

namespace CalendlyTaskAPI.AppStartUp
{
    public static class DependencyInjectionBuilder
    {
        public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IMeetingService, MeetingService>();

            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
