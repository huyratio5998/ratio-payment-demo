using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Models
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
