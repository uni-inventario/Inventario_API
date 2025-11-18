using Inventario.Core.Data;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Core.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(ContextRepository context) : base(context) { }
        public async Task<List<Produto>> GetByEstoqueIdAsync(long estoqueId, long usuarioId)
        {
            return await _context.Produtos
                .Where(p =>
                    p.EstoqueId == estoqueId &&
                    p.DeletedAt == null &&
                    p.Estoque.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Produto> GetByIdAsync(long id, long usuarioId)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.DeletedAt == null &&
                    p.Estoque.UsuarioId == usuarioId);
        }
    }
}