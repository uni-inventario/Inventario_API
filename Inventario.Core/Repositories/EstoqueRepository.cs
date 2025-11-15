using Inventario.Core.Data;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Core.Repositories
{
    public class EstoqueRepository : BaseRepository<Estoque>, IEstoqueRepository
    {
        public EstoqueRepository(ContextRepository context) : base(context) { }

        public Estoque GetByIdWithProdutosAsync(long id)
        {
            return _context.Estoques
                .Include(e => e.Produtos)
                .FirstOrDefault(e => e.Id == id);
        }

        public List<Estoque> GetByUsuarioIdAsync(long usuarioId)
        {
            return _context.Estoques
                .Include(x => x.Produtos)
                .Where(e => e.UsuarioId == usuarioId)
                .ToList();
        }
    }
}