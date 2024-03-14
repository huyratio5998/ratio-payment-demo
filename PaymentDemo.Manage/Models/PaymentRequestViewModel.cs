using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Models
{
    public class PaymentRequestViewModel
    {
        public string OrderNumber { get; set; }
        public decimal Money { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentProvider? Provider { get; set; }


        //Paypal
        public string? PayerID { get; set; }
    }
}
