using System.Linq.Expressions;

namespace EcommerceAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? Filter = null, List<String>? Include = null);

        Task<T?> GetAsync(Expression<Func<T, bool>> Filter, bool NoTracking = false, List<String>? Include = null);

        Task<T> CreateAsync(T entity);

        Task RemoveAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task SaveAsync();
    }
}

