using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Models.PaymentProviders;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentProviderFactory _paymentProviderFactory;
        private readonly ILogger _logger;

        public PaymentService(ILogger logger, IPaymentProviderFactory paymentProviderFactory)
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

            await paymentProvider.SendRequest(new BasePaymentRequestViewModel(request.Money, PaymentRequestType.RequestPayment), cancellationToken);
            _logger.LogInformation("Success to send payment request");
            return true;
        }

        public async Task<bool> ProceedRefund(PaymentRefundRequestViewModel request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start proceed refund payment");
            var paymentProvider = _paymentProviderFactory.CreatePaymentProvider(request.Provider);
            if (paymentProvider == null)
            {
                _logger.LogError("Proceed refund fail: payment provider null");
                return false;
            }

            await paymentProvider.SendRequest(new BasePaymentRequestViewModel(request.Money, PaymentRequestType.Refund), cancellationToken);
            _logger.LogInformation("Success to send refund payment request");
            return true;
        }
    }
}
