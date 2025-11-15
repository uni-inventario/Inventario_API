using Inventario.Core.Data;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;

namespace Inventario.Core.Repositories
{
    public class ProdutoRepository: BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(ContextRepository context) : base(context) { }  

    }
}