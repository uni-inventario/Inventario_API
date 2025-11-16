using System.Security.Claims;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            try
            {
                var result = await _authService.LoginAsync(req.Email, req.Senha);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var sub = User.FindFirstValue("id");
            
            if (!long.TryParse(sub, out var id)) return BadRequest();

            await _authService.LogoutAsync(id);
            return NoContent();
        }
    }
}
