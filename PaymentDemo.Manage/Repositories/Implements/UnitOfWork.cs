using Microsoft.EntityFrameworkCore.Storage;
using PaymentDemo.Manage.Data;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;

namespace PaymentDemo.Manage.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly PaymentDBContext _context;
        private Dictionary<Type, object> _repositories;
        private IDbContextTransaction _transaction;
        private bool disposedValue = false;

        public IProductRepository ProductRepository { get; private set; }        

        public UnitOfWork(PaymentDBContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
            ProductRepository = new ProductRepository(context);
        }

        public async Task CreateTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

        public IBaseRepository<T> GetRepository<T>() where T : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as BaseRepository<T>;
            }

            var repository = new BaseRepository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
