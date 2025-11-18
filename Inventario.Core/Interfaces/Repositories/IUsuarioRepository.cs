using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Core.Models;

namespace Inventario.Core.Interfaces.Repositories
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task UpdateTokenAsync(long id, string? token);
        Task<bool> CheckTokenAsync(long id, string token);
    }
}