using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Data;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;
using System.Linq.Expressions;

namespace PaymentDemo.Manage.Repositories.Implements
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly PaymentDBContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(PaymentDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return await DeleteAsync(entity);
        }

        public async Task<bool> DeleteAsync(T? entity)
        {
            if (entity == null) return false;

            _dbSet.Remove(entity);

            return true;
        }

        public async Task<bool> DeleteRangeAsync(List<T> entities)
        {
            if(entities == null) return true;            

            _dbSet.RemoveRange(entities);

            return true;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
            _dbSet.Where(expression).AsNoTracking();


        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            return Get(filter, orderBy, new IncludeDefinition<T>[0]);
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params IncludeDefinition<T>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var item in includes)
            {
                query = item.Include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public IQueryRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> referenceExpression)
        {
            return new GenericQueryRepositoryHelper<T>(this, new IncludeDefinition<T, TProperty>(referenceExpression));
        }        

        public IEnumerable<T> GetAll(bool isTracking = false)
        {
            return isTracking ? _dbSet : _dbSet.AsNoTracking();
        }

        public async Task<T?> GetByIdAsync(int id, bool isTracking = true)
        {
            if (id == 0) return null!;

            var entity = isTracking ? await _dbSet.FirstOrDefaultAsync(s => s.Id == id)
                : await _dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null) return null!;

            return entity;
        }        

        public bool Update(T entity)
        {
            if (entity == null) return false;

            entity.ModifiedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
            return true;
        }
    }
}
