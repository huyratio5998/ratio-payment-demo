using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDemo.Api.Models;
using PaymentDemo.Manage.Enums;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Api.Controllers.V1
{
    [Authorize(Roles = DemoConstant.RatioAdmin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;       

        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        /// <summary>
        /// Call back function for execute payment.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExecutePayment([FromQuery] PaymentRequestViewModel request)
        {
            var timeoutCts = new CancellationTokenSource();
            timeoutCts.CancelAfter(TimeSpan.FromMinutes(Manage.Constants.CommonConstant.MaxOrderTimeExecuteMinutes));

            var result = await _paymentService.ProceedPayment(request, timeoutCts.Token);
            if(!string.IsNullOrEmpty(request.PayerID) && result)
                await _orderService.UpdateOrderStatusByPaymentStatus(request.OrderNumber, PaymentStatus.Success.ToString());

            return Ok(result);
        }
    }
}
