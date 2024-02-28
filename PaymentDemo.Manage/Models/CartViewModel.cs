using FluentValidation;
using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Models
{
    public class CartViewModel
    {
        public CartViewModel(int id, int userId, CartStatus status, List<CartItemViewModel>? cartItems = null)
        {
            Id = id;
            UserId = userId;
            Status = status;
            CartItems = cartItems;
        }      

        public int Id { get; set; }
        public int UserId { get; set; }
        public CartStatus Status { get; set; }
        public List<CartItemViewModel>? CartItems { get; set; }
    }

    public class CartViewModelValidator : AbstractValidator<CartViewModel>
    {
        public CartViewModelValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);            
        }
    }
}
