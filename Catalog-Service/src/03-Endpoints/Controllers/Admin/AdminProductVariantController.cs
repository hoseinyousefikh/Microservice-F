using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor; // یا Admin اگر DTOهای مخصوص ادمین دارید
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin; // <-- این را اضافه کنید
using Catalog_Service.src.CrossCutting.Security;
using Catalog_Service.src.CrossCutting.Validation.Admin; // <-- این را اضافه کنید
using Catalog_Service.src.CrossCutting.Validation.Vendor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public class AdminProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminProductVariantController> _logger;

        public AdminProductVariantController(
            IProductVariantService productVariantService,
            IMapper mapper,
            ILogger<AdminProductVariantController> logger)
        {
            _productVariantService = productVariantService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductVariants(int productId)
        {
            var variants = await _productVariantService.GetByProductIdAsync(productId);
            var response = _mapper.Map<IEnumerable<ProductVariantResponse>>(variants);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductVariant(int id)
        {
            var variant = await _productVariantService.GetByIdAsync(id);
            if (variant == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<ProductVariantResponse>(variant);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductVariant([FromBody] CreateProductVariantRequest request)
        {
            var validator = new CreateProductVariantValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var variant = await _productVariantService.CreateAsync(
                request.ProductId,
                request.Sku,
                request.Name,
                _mapper.Map<Money>(request.Price), 
                _mapper.Map<Dimensions>(request.Dimensions),
                Weight.Create(request.Weight, "kg"),
                request.ImageUrl,
                _mapper.Map<Money>(request.OriginalPrice));

            var response = _mapper.Map<ProductVariantResponse>(variant);
            return CreatedAtAction(nameof(GetProductVariant), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductVariant(int id, [FromBody] UpdateProductVariantRequest request)
        {
            var validator = new UpdateProductVariantValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _productVariantService.UpdateAsync(
                id,
                request.Name,
                _mapper.Map<Money>(request.Price),
               _mapper.Map<Money?>(request.OriginalPrice),
                _mapper.Map<Dimensions>(request.Dimensions),
                Weight.Create(request.Weight, "kg"),
                request.ImageUrl);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            await _productVariantService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateProductVariant(int id)
        {
            await _productVariantService.ActivateAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateProductVariant(int id)
        {
            await _productVariantService.DeactivateAsync(id);
            return NoContent();
        }
    }
}