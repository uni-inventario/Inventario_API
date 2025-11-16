using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.Core.DTOs.Responses
{
    public class EstoqueResponseDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ProdutoResponseDto> Produtos { get; set; }
    }
}