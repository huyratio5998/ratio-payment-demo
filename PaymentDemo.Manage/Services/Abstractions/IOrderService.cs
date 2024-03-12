using PaymentDemo.Manage.Models;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IOrderService
    {
        Task<PagedResponse<OrderViewModel>> GetOrdersAsync(OrderQueryParams query);
        Task<OrderViewModel?> GetOrderAsync(int orderId, bool isTracking = true);
        Task<OrderViewModel?> GetOrderAsync(string orderNumber, bool isTracking = true);
        Task<OrderViewModel?> CreateOrderAsync(OrderViewModel newOrder, CancellationToken cancellationToken);
        Task<bool> UpdateOrderAsync(OrderViewModel newOrder);
        Task<bool> DeleteOrderAsync(string orderNumber);

        Task<bool> ShipmentTrack(string orderNumber, string orderStatus);
    }
}
