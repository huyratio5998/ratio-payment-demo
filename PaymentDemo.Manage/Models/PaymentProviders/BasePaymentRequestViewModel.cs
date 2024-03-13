using PaymentDemo.Manage.Enums;

namespace PaymentDemo.Manage.Models.PaymentProviders
{
    public class BasePaymentRequestViewModel : IPaymentRequestViewModel
    {
        public BasePaymentRequestViewModel()
        {

        }
        public BasePaymentRequestViewModel(decimal totalMoney, PaymentRequestType paymentRequestType)
        {
            TotalMoney = totalMoney;
            PaymentRequestType = paymentRequestType;
        }

        public decimal TotalMoney { get; set; }
        public PaymentRequestType PaymentRequestType { get; set; }
    }
}
