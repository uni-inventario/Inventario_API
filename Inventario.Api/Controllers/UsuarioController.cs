using System.Security.Claims;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.Interfaces.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : BaseController
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
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _usuarioHandler.GetByIdAsync(UsuarioId);
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
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _usuarioHandler.UpdateAsync(entity, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _usuarioHandler.DeleteAsync(UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}