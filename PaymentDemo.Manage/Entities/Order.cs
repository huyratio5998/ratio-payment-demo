using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Entities
{
    public class Order : BaseEntity
    {        
        public int CartId { get; set; }
        public string OrderNumber { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentProvider? PaymentProvider { get; set; }
        public ShipmentStatus ShipmentStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? OrderHistory { get; set; }
    }
}
