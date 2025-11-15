using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Core.Models;

namespace Inventario.Core.Interfaces.Repositories
{
    public interface IEstoqueRepository: IBaseRepository<Estoque>
    {
        Estoque GetByIdWithProdutosAsync(long id);
        List<Estoque> GetByUsuarioIdAsync(long usuarioId);
    }
}