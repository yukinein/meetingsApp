using CalendlyTaskAPI.Core.Consts;
using CalendlyTaskAPI.Core.DTOs.Requests;
using CalendlyTaskAPI.Core.DTOs.Responses;
using CalendlyTaskAPI.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendlyTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<AuthServiceResponse>> Register([FromForm] RegisterRequestModel requestModel)
        {
            byte[]? fileBytes = null;

            if (requestModel.Picture is not null)
            {
                using var memoryStream = new MemoryStream();

                await requestModel.Picture.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            RegisterRequest request = new()
            {
                FullName = requestModel.FullName,
                UserName = requestModel.UserName,
                Email = requestModel.Email,
                Password = requestModel.Password,
                Picture = fileBytes
            };

            return await _authService.Register(request);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthServiceResponse>> Login(LoginRequest request)
        {
            return await _authService.Login(request);
        }

        
        [HttpPost("MakeAdmin")]
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        public async Task<ActionResult<AuthServiceResponse>> MakeAdmin(UpdatePermissionRequest request)
        {
            return await _authService.MakeAdmin(request);
        }
    }
}
