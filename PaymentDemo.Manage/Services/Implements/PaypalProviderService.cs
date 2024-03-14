using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Payment _payment;
        private const string ApproveURL = "approval_url";
        private const string PaymentSession = "payment";
        private const string PaymentApproveStatus = "approved";
        private const string CallBackUrl = "/api/v1/payment/executepayment";

        public PaypalProviderService(ILogger logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> SendRequest(IPaymentRequestViewModel basePaymentRequest, CancellationToken cancelToken)
        {
            // test because haven't implement refund yet.
            if (basePaymentRequest.PaymentRequestType == PaymentRequestType.Refund) return false;

            var request = basePaymentRequest as PaypalRequestViewModel;
            if (request == null) return false;

            var payPalApiContext = GetPaypalApiContext();
            try
            {
                string paypalRedirectUrl = null;
                if (request.IsCreatePayment)
                {
                    _logger.LogInformation("Start create paypal payment: ");
                    var currentRequest = _httpContextAccessor.HttpContext?.Request;
                    string baseUrl = $"{currentRequest?.Scheme}://{currentRequest?.Host}{CallBackUrl}?";

                    _payment = CreatePayment(request, baseUrl);
                    var createdPayment = await Task.Run(() => _payment.Create(payPalApiContext));

                    var links = createdPayment.links.GetEnumerator();
                    while (links.MoveNext())
                    {
                        var currenLink = links.Current;
                        if (currenLink.rel.ToLower().Trim().Equals(ApproveURL)) paypalRedirectUrl = currenLink.href;
                    }

                    _httpContextAccessor.HttpContext?.Session.SetString(PaymentSession, createdPayment.id);
                    _logger.LogInformation("Success to create payment. Redirect url: " + paypalRedirectUrl);
                    return true;
                }
                else if (request.IsExecutePayment)
                {
                    var paymentId = _httpContextAccessor.HttpContext?.Session?.GetString(PaymentSession);
                    _payment = new Payment() { id = paymentId };
                    var executedPayment = await Task.Run(() => _payment.Execute(payPalApiContext, new PaymentExecution() { payer_id = request.PayerId }));
                    if (executedPayment.state.ToLower() != PaymentApproveStatus)
                    {
                        _logger.LogInformation("Fail to execute payment");
                        return false;
                    }

                    _logger.LogInformation("Success to execute payment");
                    return true;
                }

                return false;
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

        private Payment CreatePayment(PaypalRequestViewModel request, string baseUri)
        {            
            var redirectUrl = new RedirectUrls
            {
                cancel_url = $"{baseUri}",
                return_url = $"{baseUri}&OrderNumber={request.OrderNumber}"
            };

            var amount = new Amount
            {
                currency = "USD",
                total = request.TotalMoney.ToString()
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

        public IPaymentRequestViewModel CreateRequestModel(PaymentRequestViewModel request, PaymentRequestType paymentType)
        {
            var paymentId = _httpContextAccessor.HttpContext?.Session?.GetString(PaymentSession);
            var isCreatePayment = string.IsNullOrEmpty(paymentId);
            return new PaypalRequestViewModel()
            {
                IsCreatePayment = isCreatePayment,
                IsExecutePayment = !isCreatePayment,
                PayerId = request.PayerID ?? string.Empty,
                OrderNumber = request.OrderNumber,
                TotalMoney = request.Money,
                PaymentRequestType = paymentType
            };
        }
    }
}
