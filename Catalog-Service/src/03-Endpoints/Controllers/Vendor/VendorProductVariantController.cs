using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Vendor;
using Catalog_Service.src.CrossCutting.Exceptions;
using Catalog_Service.src.CrossCutting.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Vendor
{
    [ApiController]
    [Route("api/vendor/products/{productId}/variants")]
    [Authorize(Roles = RoleConstants.Vendor + "," + RoleConstants.SuperAdministrator)]
    public class VendorProductVariantController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IImageService _imageService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductVariantRequest> _createVariantValidator;
        private readonly IValidator<UpdateProductVariantRequest> _updateVariantValidator;

        public VendorProductVariantController(
            IProductService productService,
            IProductVariantService productVariantService,
            IImageService imageService,
            IProductAttributeService productAttributeService,
            IMapper mapper,
            IValidator<CreateProductVariantRequest> createVariantValidator,
            IValidator<UpdateProductVariantRequest> updateVariantValidator)
        {
            _productService = productService;
            _productVariantService = productVariantService;
            _imageService = imageService;
            _productAttributeService = productAttributeService;
            _mapper = mapper;
            _createVariantValidator = createVariantValidator;
            _updateVariantValidator = updateVariantValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorProductVariantResponse>>> GetVariants(
            int productId,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var variants = await _productVariantService.GetByProductIdAsync(productId, cancellationToken);
            var variantResponses = _mapper.Map<IEnumerable<VendorProductVariantResponse>>(variants);
            return Ok(variantResponses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorProductVariantResponse>> GetVariant(
            int productId,
            int id,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var variant = await _productVariantService.GetByIdAsync(id, cancellationToken);
            if (variant == null || variant.ProductId != productId)
                throw new NotFoundException("ProductVariant", id);

            var variantResponse = _mapper.Map<VendorProductVariantResponse>(variant);
            return Ok(variantResponse);
        }

        public async Task<ActionResult<VendorProductVariantResponse>> CreateVariant(
        int productId,
        [FromBody] CreateProductVariantRequest request,
        CancellationToken cancellationToken)
        {
            await _createVariantValidator.ValidateAndThrowAsync(request, cancellationToken);

            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            // Check if SKU already exists
            var existingVariant = await _productVariantService.GetBySkuAsync(request.Sku, cancellationToken);
            if (existingVariant != null)
                throw new DuplicateEntityException("ProductVariant", request.Sku);

            // Get the user ID from the JWT token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var variant = await _productVariantService.CreateAsync(
                productId,
                request.Sku,
                request.Name,
                Money.Create(request.Price, "USD"),
                Dimensions.Create(request.Dimensions.Length, request.Dimensions.Width, request.Dimensions.Height, "cm"),
                Weight.Create(request.Weight, "kg"),
                request.ImageUrl,
                request.OriginalPrice.HasValue ? Money.Create(request.OriginalPrice.Value, "USD") : null,
                cancellationToken);

            // Set image if provided
            if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                await _imageService.CreateAsync(
                    "variant-image",
                    "jpg",
                    "variant-images",
                    request.ImageUrl,
                    0, // fileSize
                    800, // width
                    600, // height
                    ImageType.Variant,
                    userId, // Pass the extracted user ID
                    null, // altText
                    true, // isPrimary
                    cancellationToken);
            }

            var variantResponse = _mapper.Map<VendorProductVariantResponse>(variant);
            return CreatedAtAction(nameof(GetVariant), new { productId, id = variant.Id }, variantResponse);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<VendorProductVariantResponse>> UpdateVariant(
            int productId,
            int id,
            [FromBody] UpdateProductVariantRequest request,
            CancellationToken cancellationToken)
        {
            await _updateVariantValidator.ValidateAndThrowAsync(request, cancellationToken);

            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var variant = await _productVariantService.GetByIdAsync(id, cancellationToken);
            if (variant == null || variant.ProductId != productId)
                throw new NotFoundException("ProductVariant", id);

            // Check if SKU already exists for another variant
            var existingVariant = await _productVariantService.GetBySkuAsync(request.Sku, cancellationToken);
            if (existingVariant != null && existingVariant.Id != id)
                throw new DuplicateEntityException("ProductVariant", request.Sku);

            await _productVariantService.UpdateAsync(
                id,
                request.Name,
                Money.Create(request.Price, "USD"),
                request.OriginalPrice.HasValue ? Money.Create(request.OriginalPrice.Value, "USD") : null,
                Dimensions.Create(request.Dimensions.Length, request.Dimensions.Width, request.Dimensions.Height, "cm"),
                Weight.Create(request.Weight, "kg"),
                request.ImageUrl,
                cancellationToken);

            var updatedVariant = await _productVariantService.GetByIdAsync(id, cancellationToken);
            var variantResponse = _mapper.Map<VendorProductVariantResponse>(updatedVariant);
            return Ok(variantResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int productId, int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var variant = await _productVariantService.GetByIdAsync(id, cancellationToken);
            if (variant == null || variant.ProductId != productId)
                throw new NotFoundException("ProductVariant", id);

            await _productVariantService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateVariant(int productId, int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var variant = await _productVariantService.GetByIdAsync(id, cancellationToken);
            if (variant == null || variant.ProductId != productId)
                throw new NotFoundException("ProductVariant", id);

            await _productVariantService.ActivateAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateVariant(int productId, int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var variant = await _productVariantService.GetByIdAsync(id, cancellationToken);
            if (variant == null || variant.ProductId != productId)
                throw new NotFoundException("ProductVariant", id);

            await _productVariantService.DeactivateAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/stock")]
        public async Task<IActionResult> UpdateVariantStock(int productId, int id, [FromBody] UpdateVariantStockRequest request, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", productId);

            var variant = await _productVariantService.GetByIdAsync(id, cancellationToken);
            if (variant == null || variant.ProductId != productId)
                throw new NotFoundException("ProductVariant", id);

            await _productVariantService.UpdateStockQuantityAsync(id, request.Quantity, cancellationToken);
            return NoContent();
        }
    }

    public class UpdateVariantStockRequest
    {
        public int Quantity { get; set; }
    }
}
