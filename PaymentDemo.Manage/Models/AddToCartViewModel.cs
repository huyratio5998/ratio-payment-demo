using FluentValidation;

namespace PaymentDemo.Manage.Models
{
    public class AddToCartViewModel
    {
        public AddToCartViewModel(int cartId, int productId, int number, decimal price)
        {
            CartId = cartId;
            ProductId = productId;
            Number = number;
            Price = price;
        }

        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Number { get; set; }
        public decimal Price { get; set; }

    }

    public class AddToCartViewModelValidator : AbstractValidator<AddToCartViewModel>
    {
        public AddToCartViewModelValidator()
        {
            RuleFor(x => x.CartId).GreaterThan(0);
        }
    }
}
