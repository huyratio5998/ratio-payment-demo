using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public PaymentProviderFactory(IHttpClientFactory httpClientFactory, ILogger<PaymentProviderFactory> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public IPaymentProvider? CreatePaymentProvider(PaymentProvider? paymentProvider)
        {
            switch (paymentProvider)
            {
                case PaymentProvider.Adyen: 
                    return new AdyenProviderService(_httpClientFactory, _logger);
                case PaymentProvider.Paypal: 
                    return new PaypalProviderService(_logger, _configuration);
                default: return null;
            }
        }
    }
}
