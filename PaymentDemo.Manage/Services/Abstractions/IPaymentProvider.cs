using PaymentDemo.Manage.Models.PaymentProviders;
using PaymentDemo.Manage.Models.PaymentProviders.Paypal;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IPaymentProvider
    {
        Task SendRequest(BasePaymentRequestViewModel paymentRequest, CancellationToken cancelToken);
    }
}
