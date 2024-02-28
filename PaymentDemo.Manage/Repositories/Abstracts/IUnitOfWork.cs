using PaymentDemo.Manage.Entities;

namespace PaymentDemo.Manage.Repositories.Abstracts
{
    public interface IUnitOfWork
    {
        Task CreateTransactionAsync();
        Task SaveAsync();
        Task CommitAsync();
        Task RollbackAsync();

        IBaseRepository<T> GetRepository<T>() where T : BaseEntity;
        IProductRepository ProductRepository { get; }        
    }
}
