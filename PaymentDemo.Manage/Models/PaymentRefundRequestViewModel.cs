using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Models
{
    public class PaymentRefundRequestViewModel
    {
        public decimal Money { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentProvider? Provider { get; set; }
    }
}
