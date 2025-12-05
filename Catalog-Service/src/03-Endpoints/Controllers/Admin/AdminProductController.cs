using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin;
using Catalog_Service.src.CrossCutting.Security;
using Catalog_Service.src.CrossCutting.Validation.Vendor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public class AdminProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminProductController> _logger;

        public AdminProductController(
            IProductService productService,
            IMapper mapper,
            ILogger<AdminProductController> logger)
        {
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] AdminProductSearchRequest request)
        {
            var validator = new AdminProductSearchValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var (products, totalCount) = await _productService.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.CategoryId,
                request.BrandId,
                request.IsActive,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.SortAscending);

            var response = new PagedResponse<AdminProductResponse>
            {
                Items = _mapper.Map<IEnumerable<AdminProductResponse>>(products),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<AdminProductResponse>(product);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var validator = new CreateProductValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var product = await _productService.CreateAsync(
                request.Name,
                request.Description,
                request.Price,
                request.BrandId,
                request.CategoryId,
                request.Sku,
                _mapper.Map<Dimensions>(request.Dimensions),
                new Weight(request.Weight, "kg"),
                request.ImageUrl,
                request.OriginalPrice,
                request.MetaTitle,
                request.MetaDescription);

            var response = _mapper.Map<AdminProductResponse>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
        {
            var validator = new UpdateProductValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _productService.UpdateAsync(
                id,
                request.Name,
                request.Description,
                request.Price,
                request.OriginalPrice,
                _mapper.Map<Dimensions>(request.Dimensions),
                new Weight(request.Weight, "kg"),
                request.ImageUrl,
                request.MetaTitle,
                request.MetaDescription);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishProduct(int id)
        {
            await _productService.PublishAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/unpublish")]
        public async Task<IActionResult> UnpublishProduct(int id)
        {
            await _productService.UnpublishAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/archive")]
        public async Task<IActionResult> ArchiveProduct(int id)
        {
            await _productService.ArchiveAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/feature")]
        public async Task<IActionResult> SetAsFeatured(int id)
        {
            await _productService.SetAsFeaturedAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}/feature")]
        public async Task<IActionResult> RemoveFromFeatured(int id)
        {
            await _productService.RemoveFromFeaturedAsync(id);
            return NoContent();
        }
    }
}
