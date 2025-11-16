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
            return _context.Estoques
                .Include(e => e.Produtos)
                .FirstOrDefault(e => e.Id == id
                    && e.UsuarioId == usuarioId
                    && e.DeletedAt == null);
        }

        public async Task<List<Estoque>> GetAllAsync(long usuarioId)
        {
            return _context.Estoques
                .Include(x => x.Produtos)
                .Where(e => e.UsuarioId == usuarioId
                    && e.DeletedAt == null)
                .ToList();
        }
    }
}