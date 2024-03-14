using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDemo.Api.Models;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Api.Controllers.V1
{
    [Authorize(Roles = DemoConstant.RatioAdmin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = DemoConstant.RatioAdmin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _orderService.GetOrderAsync(id, false);
            if (result == null || result.Id == null || result.Id == 0) return NotFound();

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<IActionResult> Gets([FromQuery] string? q, [FromQuery] int page, [FromQuery] int pageSize)
        {
            OrderQueryParams queryParams = new OrderQueryParams()
            {
                SearchText = q?.Trim(),
                PageNumber = page == 0 ? CommonConstant.PageIndexDefault : page,
                PageSize = pageSize == 0 ? CommonConstant.PageSizeDefault : pageSize
            };

            var result = await _orderService.GetOrdersAsync(queryParams);
            if (result == null || result.Items == null || result.Items.Count() == 0) return NotFound();

            return Ok(result);
        }

        [Authorize(Roles = DemoConstant.RatioAdmin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderViewModel request)
        {
            var timeoutCts = new CancellationTokenSource();
            timeoutCts.CancelAfter(TimeSpan.FromMinutes(Manage.Constants.CommonConstant.MaxOrderTimeExecuteMinutes));

            var result = await _orderService.CreateOrderAsync(request, timeoutCts.Token);
            if (result == null || result.Id == 0) return BadRequest();

            return Ok(result);
        }

        [Authorize(Roles = DemoConstant.RatioAdmin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string orderNumber)
        {
            var result = await _orderService.DeleteOrderAsync(orderNumber);
            if (!result) return Ok("Delete Failure!");

            return Ok(result);
        }

        [Authorize(Roles = DemoConstant.RatioAdmin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> ShipmentTrack([FromQuery] string orderNumber, [FromQuery] string shipmentStatus)
        {
            if (string.IsNullOrWhiteSpace(orderNumber) || string.IsNullOrWhiteSpace(shipmentStatus)) return BadRequest();

            var result = await _orderService.ShipmentTrack(orderNumber, shipmentStatus);
            return Ok(result);
        }
    }
}
