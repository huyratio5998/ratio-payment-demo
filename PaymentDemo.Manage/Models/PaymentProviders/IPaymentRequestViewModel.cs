using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Models.PaymentProviders
{
    public interface IPaymentRequestViewModel
    {        
        public decimal TotalMoney { get; set; }
        public PaymentRequestType PaymentRequestType { get; set; }
    }
}
