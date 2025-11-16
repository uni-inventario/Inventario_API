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
    public class ProdutoController : BaseController
    {
        private readonly IProdutoHandler _produtoHandler;

        public ProdutoController(IProdutoHandler produtoHandler)
        {
            _produtoHandler = produtoHandler;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });


                var entry = await _produtoHandler.GetByIdAsync(id, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRangeAsync([FromBody] List<ProdutoRequestDto?> entities)
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _produtoHandler.AddRangeAsync(entities, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] ProdutoRequestDto? entity)
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _produtoHandler.UpdateAsync(entity, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            try
            {
                if (UsuarioId == null)
                    return Unauthorized(new { Errors = new List<string> { "Usuário não autenticado." } });

                var entry = await _produtoHandler.DeleteAsync(id, UsuarioId.Value);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}