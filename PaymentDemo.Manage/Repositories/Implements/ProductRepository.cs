using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Data;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;

namespace PaymentDemo.Manage.Repositories.Implements
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(PaymentDBContext context) : base(context)
        {
        }

        public async Task<Product?> GetByIdIncludeAsync(int id, bool isTracking = true)
        {
            var result = GetAll(isTracking).AsQueryable()
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            return await result;
        }
    }
}
