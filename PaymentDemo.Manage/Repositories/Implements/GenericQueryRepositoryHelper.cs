using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;
using System.Linq.Expressions;

namespace PaymentDemo.Manage.Repositories.Implements
{
    public class GenericQueryRepositoryHelper<T> : IQueryRepository<T>
    where T : BaseEntity
    {
        private readonly IList<IncludeDefinition<T>> _includeDefinitions;
        private readonly IBaseRepository<T> _repository;

        internal GenericQueryRepositoryHelper(IBaseRepository<T> repository, IncludeDefinition<T> includeDefinition)
        {
            _repository = repository;
            _includeDefinitions = new List<IncludeDefinition<T>> { includeDefinition };
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            return _repository.Get(filter, orderBy, _includeDefinitions.ToArray());
        }

        public IQueryRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> referenceExpression)
        {
            _includeDefinitions.Add(new IncludeDefinition<T, TProperty>(referenceExpression));
            return this;
        }
    }
}
