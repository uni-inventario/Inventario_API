using Inventario.Core.Data;
using Inventario.Core.Interfaces.Repositories;

namespace Inventario.Core.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public readonly ContextRepository _context;

        public BaseRepository(ContextRepository context)
        {
            _context = context;
        }

        public void AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

        public T GetByIdAsync(long id)
        {
            return _context.Set<T>().Find(id);
        }

        public void AddRangeAsync(List<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            _context.SaveChanges();
        }
    }
}