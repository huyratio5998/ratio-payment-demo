using PaymentDemo.Manage.Entities;

namespace PaymentDemo.Manage.Repositories.Abstracts
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product?> GetByIdIncludeAsync(int id, bool isTracking = true);
    }
}
