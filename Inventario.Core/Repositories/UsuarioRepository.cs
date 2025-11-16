using Inventario.Core.Data;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Core.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ContextRepository context) : base(context) { }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null);
        }

        public async Task UpdateTokenAsync(long id, string? token)
        {
            var usuario = await _context.Usuarios.FirstAsync(c => c.Id == id);
            usuario.UpdatedAt = DateTime.Now;
            usuario.Token = token;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckTokenAsync(long id, string token)
        {
            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            if (usuario is null) return false;
            return usuario.Token == token;
        }


    }
}