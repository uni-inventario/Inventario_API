namespace Inventario.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        public void AddAsync(T entity);
        void UpdateAsync(T entity);
        T GetByIdAsync(long id);
        void AddRangeAsync(List<T> entities);
    }
}