using Inventario.Core.Data;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Core.Repositories
{
    public class EstoqueRepository : BaseRepository<Estoque>, IEstoqueRepository
    {
        public EstoqueRepository(ContextRepository context) : base(context) { }

        public async Task<Estoque> GetByIdAsync(long id, long usuarioId)
        {
            var estoque = await _context.Estoques
                .Include(e => e.Produtos)
                .FirstOrDefaultAsync(e =>
                    e.Id == id &&
                    e.UsuarioId == usuarioId &&
                    e.DeletedAt == null
                );

            if (estoque == null)
                return null;

            estoque.Produtos = estoque.Produtos
                .Where(p => p.DeletedAt == null)
                .ToList();

            return estoque;
        }

        public async Task<List<Estoque>> GetAllAsync(long usuarioId)
        {
            var estoques = await _context.Estoques
                .Include(e => e.Produtos)
                .Where(e =>
                    e.UsuarioId == usuarioId &&
                    e.DeletedAt == null
                )
                .ToListAsync();

            foreach (var estoque in estoques)
            {
                estoque.Produtos = estoque.Produtos
                    .Where(p => p.DeletedAt == null)
                    .ToList();
            }

            return estoques;
        }

    }
}