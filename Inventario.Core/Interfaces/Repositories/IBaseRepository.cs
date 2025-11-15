namespace Inventario.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> GetByIdAsync(long id);
        Task AddRangeAsync(List<T> entities);
    }
}