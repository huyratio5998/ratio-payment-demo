using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Models.PaymentProviders
{
    public class BasePaymentRequestViewModel : IPaymentRequestViewModel
    {       
        public string OrderNumber { get; set; }
        public decimal TotalMoney { get; set; }
        public PaymentRequestType PaymentRequestType { get; set; }
    }
}
