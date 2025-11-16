using System.Security.Claims;
using Inventario.Api.Controllers;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
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
            if (UsuarioId == null)
                return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

            await _authService.LogoutAsync(UsuarioId.Value);
            return NoContent();
        }
    }
}
