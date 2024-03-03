using PaymentDemo.Manage.Models;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IOrderService
    {
        Task<PagedResponse<OrderViewModel>> GetOrdersAsync(OrderQueryParams query);
        Task<OrderViewModel?> GetOrderAsync(int orderId);
        Task<OrderViewModel> CreateOrderAsync(OrderViewModel newOrder);
        Task<bool> UpdateOrderAsync(OrderViewModel newOrder);
        Task<bool> DeleteOrderAsync(string orderNumber);
    }
}
