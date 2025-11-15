using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.Core.DTOs.Requests
{
    public class ProdutoRequestDto
    {
        public long? Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal? Preco { get; set; }
        public int? Quantidade { get; set; }
        public long? EstoqueId { get; set; }
    }
}