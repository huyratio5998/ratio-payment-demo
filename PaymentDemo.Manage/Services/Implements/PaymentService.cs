using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentProviderFactory _paymentProviderFactory;
        private readonly ILogger _logger;

        public PaymentService(ILogger<PaymentService> logger, IPaymentProviderFactory paymentProviderFactory)
        {
            _logger = logger;
            _paymentProviderFactory = paymentProviderFactory;
        }

        public async Task<bool> ProceedPayment(PaymentRequestViewModel request, CancellationToken cancellationToken)
        {
            if (request.PaymentType == PaymentType.COD || request.Money == 0) return true;

            _logger.LogInformation("Start proceed payment online");
            var paymentProvider = _paymentProviderFactory.CreatePaymentProvider(request.Provider);
            if (paymentProvider == null)
            {
                _logger.LogError("Proceed payment fail: payment provider null");
                return false;
            }

            var result = await paymentProvider.SendRequest(paymentProvider.CreateRequestModel(request, PaymentRequestType.RequestPayment), cancellationToken);
            _logger.LogInformation("End proceed payment online");
            return result;
        }

        public async Task<bool> ProceedRefund(PaymentRequestViewModel request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start proceed refund payment");
            var paymentProvider = _paymentProviderFactory.CreatePaymentProvider(request.Provider);
            if (paymentProvider == null)
            {
                _logger.LogError("Proceed refund fail: payment provider null");
                return false;
            }

            var result = await paymentProvider.SendRequest(paymentProvider.CreateRequestModel(request, PaymentRequestType.Refund), cancellationToken);
            _logger.LogInformation("End proceed refund online");
            return result;           
        }
    }
}
