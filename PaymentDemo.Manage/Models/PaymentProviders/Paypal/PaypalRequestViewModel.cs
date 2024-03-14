namespace PaymentDemo.Manage.Models.PaymentProviders.Paypal
{
    public class PaypalRequestViewModel: BasePaymentRequestViewModel
    {
        public bool IsCreatePayment { get; set; }
        public bool IsExecutePayment { get; set; }
        public string PayerId { get; set; }
    }
}
