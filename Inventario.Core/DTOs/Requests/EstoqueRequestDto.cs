using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.Core.DTOs.Requests
{
    public class EstoqueRequestDto
    {
        public long? Id { get; set; }
        public string? Nome { get; set; }
        public long? UsuarioId { get; set; }
    }
}