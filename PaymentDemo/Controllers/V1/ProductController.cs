using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PaymentDemo.Api.Models;
using PaymentDemo.Manage;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Services.Abstractions;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace PaymentDemo.Api.Controllers.V1
{
    [Authorize(Roles =$"{DemoConstant.RatioAdmin},{DemoConstant.RatioReadOnly}")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IDistributedCache distributedCache)
        {
            _productService = productService;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [Authorize(Roles = DemoConstant.RatioAdmin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            ProductViewModel? result = null;
            var cacheKey = $"v1-{nameof(ProductController)}-{nameof(Get)}-{id}";
            var redisResult = await _distributedCache.GetAsync(cacheKey);

            if (redisResult != null)
                result = JsonSerializer.Deserialize<ProductViewModel>(Encoding.UTF8.GetString(redisResult));
            else
            {
                result = await _productService.GetProductAsync(id, false);
                var option = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                await _distributedCache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result)), option);
            }

            if (result == null || result.Id == null || result.Id == 0) return NotFound();

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<IActionResult> Gets([FromQuery] string? q, [FromQuery] int page, [FromQuery] int pageSize)
        {
            PagedResponse<ProductViewModel>? results = null;
            var cacheKey = $"v1-{nameof(ProductController)}-{nameof(Gets)}-{q}-{page}-{pageSize}";
            var redisResult = await _distributedCache.GetAsync(cacheKey);

            if (redisResult != null)
            {
                _logger.LogInformation($"Get products from cache");
                results = JsonSerializer.Deserialize<PagedResponse<ProductViewModel>>(Encoding.UTF8.GetString(redisResult));
            }
            else
            {
                ProductQueryParams queryParams = new ProductQueryParams()
                {
                    SearchText = q?.Trim(),
                    PageNumber = page == 0 ? CommonConstant.PageIndexDefault : page,
                    PageSize = pageSize == 0 ? CommonConstant.PageSizeDefault : pageSize
                };
                _logger.LogInformation($"Start query: {JsonSerializer.Serialize(queryParams)}");

                results = await _productService.GetProductsAsync(queryParams);
                if (results == null || results.Items == null || results.Items.Count() == 0) return NotFound();

                _logger.LogInformation("Finish request");

                var option = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                await _distributedCache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(results)), option);
            }

            return Ok(results);
        }

        [Authorize(Roles =DemoConstant.RatioAdmin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductViewModel request)
        {
            if (!string.IsNullOrWhiteSpace(request.ProductCategoriesJson)) 
                request.ProductCategories = JsonSerializer.Deserialize<List<CategoryViewModel>>(request.ProductCategoriesJson);

            var result = await _productService.CreateProductAsync(request);
            if (result == 0) return BadRequest();

            return CreatedAtAction(nameof(Get), new { id = result }, result);
        }

        [Authorize(Roles = DemoConstant.RatioAdmin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductViewModel request)
        {
            if (!string.IsNullOrWhiteSpace(request.ProductCategoriesJson))
                request.ProductCategories = JsonSerializer.Deserialize<List<CategoryViewModel>>(request.ProductCategoriesJson);

            var result = await _productService.UpdateProductAsync(request);
            if (!result) return BadRequest();

            return Ok(result);
        }

        [Authorize(Roles = DemoConstant.RatioAdmin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            if(id <=0) return BadRequest();

            var result = await _productService.DeleteProductAsync(id);
            if (!result) return Ok("Delete Failure!");

            return Ok(result);
        }
    }
}
