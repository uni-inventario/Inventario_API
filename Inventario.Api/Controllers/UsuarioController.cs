using System.Security.Claims;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.Interfaces.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioHandler _usuarioHandler;

        public UsuarioController(IUsuarioHandler usuarioHandler)
        {
            _usuarioHandler = usuarioHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var sub = User.FindFirstValue("id");
                if (!long.TryParse(sub, out var id)) return Unauthorized();

                var entry = await _usuarioHandler.GetByIdAsync(id);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] UsuarioRequestDto? entity)
        {
            try
            {
                var entry = await _usuarioHandler.AddAsync(entity);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UsuarioRequestDto? entity)
        {
            try
            {
                var sub = User.FindFirstValue("id");
                if (!long.TryParse(sub, out var userId)) return Unauthorized();

                var entry = await _usuarioHandler.UpdateAsync(entity, userId);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}