using FluentValidation;
using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Models
{
    public class OrderViewModel
    {
        public CartViewModel Cart { get; set; }
        public UserViewModel User { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public PaymentType PaymentType { get; set; }
        public ShipmentStatus ShipmentStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public class OrderViewModelValidator : AbstractValidator<OrderViewModel>
    {
        public OrderViewModelValidator()
        {
            RuleFor(x => x.Cart).NotNull();
            RuleFor(x=>x.User).NotNull();
            RuleFor(x=>x.ShippingAddress).NotEmpty();
            RuleFor(x=>x.PhoneNumber).NotEmpty();
        }
    }
}
