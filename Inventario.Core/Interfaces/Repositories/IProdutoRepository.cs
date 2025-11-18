using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Core.Models;

namespace Inventario.Core.Interfaces.Repositories
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Task<List<Produto>> GetByEstoqueIdAsync(long estoqueId, long usuarioId);
        Task<Produto> GetByIdAsync(long id, long usuarioId);
    }
}