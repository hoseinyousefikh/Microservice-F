using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Public;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using Catalog_Service.src._03_Endpoints.DTOs.Responses;
using Catalog_Service.src._03_Endpoints.DTOs.Responses;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin;
using Catalog_Service.src.CrossCutting.Security;
using Catalog_Service.src.CrossCutting.Validation.Admin;
using Catalog_Service.src.CrossCutting.Validation.Public;
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
            var productSearchRequest = new ProductSearchRequest
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SearchTerm = request.SearchTerm,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                SortBy = request.SortBy,
                SortAscending = request.SortAscending
            };

            var validator = new ProductSearchValidator();
            var validationResult = await validator.ValidateAsync(productSearchRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            ProductStatus? status = null;
            if (request.IsActive.HasValue)
            {
                status = request.IsActive.Value ? ProductStatus.Published : ProductStatus.Draft;
            }

            var result = await _productService.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.CategoryId,
                request.BrandId,
                status,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.SortAscending,
                default); // اضافه کردن CancellationToken

            var response = new PagedResponse<AdminProductResponse>
            {
                Items = _mapper.Map<IEnumerable<AdminProductResponse>>(result.Products),
                TotalCount = result.TotalCount,
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
                _mapper.Map<Money>(request.Price),
                request.BrandId,
                request.CategoryId,
                request.Sku,
                _mapper.Map<Dimensions>(request.Dimensions),
                Weight.Create(request.Weight, "kg"),
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
                _mapper.Map<Money>(request.Price),
                _mapper.Map<Money?>(request.OriginalPrice),
                _mapper.Map<Dimensions>(request.Dimensions),
                Weight.Create(request.Weight, "kg"),
                request.MetaTitle,
                request.MetaDescription,
                default); // اضافه کردن CancellationToken

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