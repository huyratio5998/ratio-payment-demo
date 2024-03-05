using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace PaymentDemo.Manage.Repositories.Abstracts
{
    public abstract class IncludeDefinition<T>
    {
        public abstract IQueryable<T> Include(IQueryable<T> entities);
    }

    public class IncludeDefinition<T, TProperty> : IncludeDefinition<T>
    {
        public IncludeDefinition(Expression<Func<T, TProperty>> includeEx)
        {
            _includeEx = includeEx;
        }

        private readonly Expression<Func<T, TProperty>> _includeEx;

        public override IQueryable<T> Include(IQueryable<T> entities)
        {
            return entities.Include(_includeEx);
        }
    }
}
