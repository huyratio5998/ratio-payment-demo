using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Models.PaymentProviders;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IPaymentProvider
    {
        Task<bool> SendRequest(IPaymentRequestViewModel paymentRequest, CancellationToken cancelToken);
        IPaymentRequestViewModel CreateRequestModel(PaymentRequestViewModel request, PaymentRequestType paymentType);
    }
}
