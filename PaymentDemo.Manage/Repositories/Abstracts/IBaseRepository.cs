using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;
using System.Linq.Expressions;

namespace PaymentDemo.Manage
{
    public interface IBaseRepository<T> : IQueryRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll(bool isTracking = false);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params IncludeDefinition<T>[] includes);        
        Task<T?> GetByIdAsync(int id, bool isTracking = true);
        Task<T> CreateAsync(T entity);
        bool Update(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(T? entity);
        Task<bool> DeleteRangeAsync(List<T> entities);
    }
}