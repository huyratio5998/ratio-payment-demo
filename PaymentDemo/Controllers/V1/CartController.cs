using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDemo.Api.Models;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Api.Controllers.V1
{
    [Authorize(Roles =DemoConstant.RatioAdmin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _cartService.GetCartAsync(id, false);
            if (result == null || result.Id == null || result.Id == 0) return NotFound();

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart([FromBody] CartViewModel request)
        {
            var result = await _cartService.AddToCartAsync(request);
            if (!result) return BadRequest();

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeCartItem([FromBody] AddToCartViewModel request)
        {
            var result = await _cartService.ChangeCartItemAsync(request);
            if (!result) return BadRequest();

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete]
        public async Task<IActionResult> DeleteCart([FromQuery] int? userId, [FromQuery] int? cartId)
        {
            var result = await _cartService.DeleteCartAsync(userId, cartId);
            if (!result) return BadRequest();

            return Ok(result);
        }
    }
}
