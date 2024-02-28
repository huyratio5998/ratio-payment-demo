using PaymentDemo.Manage.Models;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface ICartService
    {
        Task<bool> AddToCartAsync(CartViewModel cart);
        Task<bool> ChangeCartItemAsync(AddToCartViewModel cartItem);
        Task<bool> DeleteCartAsync(int? userId, int? cartId);

    }
}
