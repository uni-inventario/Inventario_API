using Inventario.Core.Data;
using Inventario.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Core.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public readonly ContextRepository _context;

        public BaseRepository(ContextRepository context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _context.Set<T>()
                .FirstOrDefaultAsync(e => EF.Property<long>(e, "Id") == id
                && EF.Property<DateTime?>(e, "DeletedAt") == null);
        }

        public async Task AddRangeAsync(List<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}