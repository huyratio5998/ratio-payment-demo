using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models.PaymentProviders;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class AdyenProviderService : IPaymentProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public AdyenProviderService(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task SendRequest(BasePaymentRequestViewModel basePaymentRequest, CancellationToken cancelToken)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient(PaymentProvider.Paypal.ToString());
                // add authorize
                // add request
                await httpClient.SendAsync(new HttpRequestMessage(), cancelToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Fail to payment: " + ex.ToString());
                throw ex;
            }
        }
    }
}
