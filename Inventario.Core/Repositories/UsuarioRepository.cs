using Inventario.Core.Data;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;

namespace Inventario.Core.Repositories
{
    public class UsuarioRepository: BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ContextRepository context) : base(context) { }

    }
}