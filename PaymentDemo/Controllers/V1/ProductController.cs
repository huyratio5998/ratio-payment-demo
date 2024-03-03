using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
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
        public async Task<IActionResult> Get([FromQuery] int id)
        {
            var result = await _productService.GetProductAsync(id, false);
            if (result == null || result.Id == null || result.Id == 0) return NotFound();

            return Ok(result);
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

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductViewModel request)
        {
            var result = await _productService.CreateProductAsync(request);
            if (result == 0) return BadRequest();

            return CreatedAtAction(nameof(Get),new {id = result});
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] ProductViewModel request)
        {
            var result = await _productService.UpdateProductAsync(request);
            if (!result) return BadRequest();

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            if(id <=0) return BadRequest();

            var result = await _productService.DeleteProductAsync(id);
            if (!result) return Ok("Delete Failure!");

            return Ok(result);
        }
    }
}
