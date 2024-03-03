using PaymentDemo.Manage.Models;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IProductService
    {
        Task<ProductViewModel> GetProductAsync(int productId, bool isTracking = true);
        Task<PagedResponse<ProductViewModel>> GetProductsAsync(ProductQueryParams query);
        Task<int> CreateProductAsync(ProductViewModel newProduct);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> UpdateProductAsync(ProductViewModel newProduct);
    }
}
