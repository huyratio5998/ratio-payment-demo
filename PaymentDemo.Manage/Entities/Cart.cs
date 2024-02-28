using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Entities
{
    public class Cart : BaseEntity
    {
        public int UserId { get; set; }
        public CartStatus Status { get; set; }
        public List<ProductCart> ProductCarts { get; set; }
    }
}
