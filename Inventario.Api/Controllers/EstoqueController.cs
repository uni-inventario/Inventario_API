using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Core.DTOs.Requests;
using Inventario.Core.Interfaces.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : BaseController
    {
        private readonly IEstoqueHandler _estoqueHandler;

        public EstoqueController(IEstoqueHandler estoqueHandler)
        {
            _estoqueHandler = estoqueHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _estoqueHandler.GetAllAsync(UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _estoqueHandler.GetByIdAsync(id, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] EstoqueRequestDto entity)
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _estoqueHandler.AddAsync(entity, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] EstoqueRequestDto entity)
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _estoqueHandler.UpdateAsync(entity, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] long id)
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _estoqueHandler.DeleteAsync(id, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    
    }
}