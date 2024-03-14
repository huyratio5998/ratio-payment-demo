using PaymentDemo.Manage.Models;

namespace PaymentDemo.Manage.Services.Abstractions
{
    public interface IPaymentService
    {
        Task<bool> ProceedPayment(PaymentRequestViewModel request, CancellationToken cancellationToken);
        Task<bool> ProceedRefund(PaymentRequestViewModel request, CancellationToken cancellationToken);
    }
}
