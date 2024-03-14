using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentProviderFactory(ILogger<PaymentProviderFactory> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public IPaymentProvider? CreatePaymentProvider(PaymentProvider? paymentProvider)
        {
            switch (paymentProvider)
            {
                case PaymentProvider.Adyen: 
                    return new AdyenProviderService(_httpClientFactory, _logger);
                case PaymentProvider.Paypal: 
                    return new PaypalProviderService(_logger, _configuration, _httpContextAccessor);
                default: return null;
            }
        }
    }
}
