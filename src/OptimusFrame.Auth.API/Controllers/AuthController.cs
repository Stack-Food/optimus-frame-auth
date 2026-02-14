using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OptimusFrame.Auth.Application.Interfaces;

namespace OptimusFrame.Auth.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IIdentityProvider authService) : ControllerBase
    {
        private readonly IIdentityProvider _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var userId = await _authService.RegisterAsync(request.Email, request.Password);
            return Ok(new { userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(new { token });
        }
    }

}
