using FluentValidation;

namespace PaymentDemo.Manage.Models
{
    public class CartItemViewModel
    {
        public CartItemViewModel(int productId, int number, decimal price)
        {
            ProductId = productId;
            Number = number;
            Price = price;
        }

        public int ProductId { get; set; }
        public int Number { get; set; }
        public decimal Price { get; set; }
    }

    public class CartItemViewModelValidator : AbstractValidator<CartItemViewModel>
    {
        public CartItemViewModelValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Number).GreaterThan(0);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
}
