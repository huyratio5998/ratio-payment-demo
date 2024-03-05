using PaymentDemo.Manage.Entities;
using System.Linq.Expressions;

namespace PaymentDemo.Manage.Repositories.Abstracts
{    
    public interface IQueryRepository<T>
     where T : BaseEntity
    {
        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        IQueryRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> referenceExpression);
    }
}
