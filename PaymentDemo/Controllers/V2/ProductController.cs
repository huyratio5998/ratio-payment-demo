using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Api.Controllers.V2
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<IActionResult> Gets([FromQuery] string? q, [FromQuery] int page, [FromQuery] int pageSize)
        {
            ProductQueryParams queryParams = new ProductQueryParams()
            {
                SearchText = q?.Trim(),
                PageNumber = page == 0 ? CommonConstant.PageIndexDefault : page,
                PageSize = pageSize == 0 ? CommonConstant.PageSizeDefault : pageSize
            };

            var result = await _productService.GetProductsAsync(queryParams);
            if (result == null || result.Items == null || result.Items.Count() == 0) return NotFound();

            return Ok(result);
        }
    }
}
