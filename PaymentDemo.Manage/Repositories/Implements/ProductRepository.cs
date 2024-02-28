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
    }
}
