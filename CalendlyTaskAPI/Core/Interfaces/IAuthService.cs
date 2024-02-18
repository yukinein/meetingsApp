using CalendlyTaskAPI.Core.DTOs.Requests;
using CalendlyTaskAPI.Core.DTOs.Responses;

namespace CalendlyTaskAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthServiceResponse> Register(RegisterRequest request);
        Task<AuthServiceResponse> Login(LoginRequest request);
        Task<AuthServiceResponse> MakeAdmin(UpdatePermissionRequest request);
    }
}
