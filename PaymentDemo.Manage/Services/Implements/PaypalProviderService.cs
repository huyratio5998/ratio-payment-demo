using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models.PaymentProviders;
using PaymentDemo.Manage.Models.PaymentProviders.Paypal;
using PaymentDemo.Manage.Services.Abstractions;
using PayPal.Api;

namespace PaymentDemo.Manage.Services.Implements
{
    public class PaypalProviderService : IPaymentProvider
    {        
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private Payment _payment;        

        public PaypalProviderService(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;            
        }

        public async Task SendRequest(BasePaymentRequestViewModel basePaymentRequest, CancellationToken cancelToken)
        {
            //PaypalRequestViewModel request
            try
            {
                // create payment
                var payPalApiContext = GetPaypalApiContext();
                _payment = CreatePayment(basePaymentRequest.TotalMoney);
                var createdPayment = _payment.Create(payPalApiContext);

                //if(createdPayment != null && createdPayment.links) { }
                //// execute payment

                //_payment.Execute(apiContext, new PaymentExecution() { payer_id = ""});
                                               
            }
            catch (Exception ex)
            {
                _logger.LogError("Fail to payment: " + ex.ToString());
                throw ex;
            }            
        }

        private APIContext GetPaypalApiContext()
        {
            var clientId = _configuration.GetValue<string>("PayPal:ClientId");
            var clientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:Mode");

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, new Dictionary<string, string>() { { "mode", mode } }).GetAccessToken();
            var payPalContext = new APIContext(accessToken);
            payPalContext.Config = new Dictionary<string, string>() { { "mode", mode } };

            return payPalContext;
        }

        private Payment CreatePayment(decimal totalMoney)
        {
            var redirectUrl = new RedirectUrls
            {
                cancel_url = "",
                return_url = ""
            };

            var amount = new Amount
            {
                currency = "USD",
                total = totalMoney.ToString()
            };

            var transaction = new List<Transaction>();
            transaction.Add(new Transaction
            {
                description = "create payment paypal",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount
            });

            return new Payment()
            {
                intent = "sale",
                payer = new Payer() { payment_method = "paypal" },
                transactions = transaction,
                redirect_urls = redirectUrl
            };
        }
    }
}
