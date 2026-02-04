using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Vendor;
using Catalog_Service.src._03_Endpoints.DTOs.Responses;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Vendor;
using Catalog_Service.src.CrossCutting.Exceptions;
using Catalog_Service.src.CrossCutting.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Vendor
{
    [ApiController]
    [Route("api/vendor/products")]
    //[Authorize(Roles = RoleConstants.Vendor + "," + RoleConstants.SuperAdministrator)]
    public class VendorProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IImageService _imageService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductTagService _productTagService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductRequest> _createProductValidator;
        private readonly IValidator<UpdateProductRequest> _updateProductValidator;
        private readonly IValidator<VendorProductSearchRequest> _searchValidator;

        public VendorProductController(
            IProductService productService,
            IProductVariantService productVariantService,
            IImageService imageService,
            IProductAttributeService productAttributeService,
            IProductTagService productTagService,
            IMapper mapper,
            IValidator<CreateProductRequest> createProductValidator,
            IValidator<UpdateProductRequest> updateProductValidator,
            IValidator<VendorProductSearchRequest> searchValidator)
        {
            _productService = productService;
            _productVariantService = productVariantService;
            _imageService = imageService;
            _productAttributeService = productAttributeService;
            _productTagService = productTagService;
            _mapper = mapper;
            _createProductValidator = createProductValidator;
            _updateProductValidator = updateProductValidator;
            _searchValidator = searchValidator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<VendorProductResponse>>> GetProducts(
            [FromQuery] VendorProductSearchRequest request,
            CancellationToken cancellationToken)
        {
            await _searchValidator.ValidateAndThrowAsync(request, cancellationToken);

            var (products, totalCount) = await _productService.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.CategoryId,
                request.BrandId,
                null, // status
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.SortAscending,
                cancellationToken);

            var productResponses = _mapper.Map<IEnumerable<VendorProductResponse>>(products);

            return Ok(new PagedResponse<VendorProductResponse>
            {
                Items = productResponses,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorProductResponse>> GetProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            var productResponse = _mapper.Map<VendorProductResponse>(product);
            return Ok(productResponse);
        }

        [HttpPost]
        public async Task<ActionResult<VendorProductResponse>> CreateProduct(
            [FromBody] CreateProductRequest request,
            CancellationToken cancellationToken)
        {
            await _createProductValidator.ValidateAndThrowAsync(request, cancellationToken);

            if (await _productService.ExistsBySkuAsync(request.Sku, cancellationToken))
                throw new DuplicateEntityException($"A product with SKU '{request.Sku}' already exists.");

            if (request.Images.Count > 10)
                return BadRequest("You can upload a maximum of 10 images.");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var product = await _productService.CreateAsync(
                request.Name,
                request.Description,
                Money.Create(request.Price, "USD"),
                request.BrandId,
                request.CategoryId,
                request.Sku,
                Dimensions.Create(
                    request.Dimensions.Length,
                    request.Dimensions.Width,
                    request.Dimensions.Height,
                    "cm"),
                Weight.Create(request.Weight, "kg"),
                userId,
                request.MetaTitle,
                request.MetaDescription,
                request.Images,
                cancellationToken);

            if (request.OriginalPrice.HasValue)
            {
                await _productService.UpdateAsync(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    Money.Create(request.OriginalPrice.Value, "USD"),
                    product.Dimensions,
                    product.Weight,
                    product.MetaTitle,
                    product.MetaDescription,
                    cancellationToken);
            }

            var productResponse = _mapper.Map<VendorProductResponse>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productResponse);
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<VendorProductResponse>> UpdateProduct(
            int id,
            [FromBody] UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            await _updateProductValidator.ValidateAndThrowAsync(request, cancellationToken);

            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            // Check if SKU already exists for another product
            if (await _productService.ExistsBySkuAsync(request.Sku, cancellationToken) && product.Sku != request.Sku)
            {
                throw new DuplicateEntityException($"A product with SKU '{request.Sku}' already exists.");
            }

            // Update product details. The UpdateAsync method in ProductService handles slug update if the name changes.
            await _productService.UpdateAsync(
                id,
                request.Name,
                request.Description,
                Money.Create(request.Price, "USD"),
                request.OriginalPrice.HasValue ? Money.Create(request.OriginalPrice.Value, "USD") : null,
                Dimensions.Create(request.Dimensions.Length, request.Dimensions.Width, request.Dimensions.Height, "cm"),
                Weight.Create(request.Weight, "kg"),
                request.MetaTitle,
                request.MetaDescription,
                cancellationToken);

            var updatedProduct = await _productService.GetByIdAsync(id, cancellationToken);
            var productResponse = _mapper.Map<VendorProductResponse>(updatedProduct);
            return Ok(productResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            await _productService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            await _productService.PublishAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/unpublish")]
        public async Task<IActionResult> UnpublishProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            await _productService.UnpublishAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/archive")]
        public async Task<IActionResult> ArchiveProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            await _productService.ArchiveAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/feature")]
        public async Task<IActionResult> FeatureProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            await _productService.SetAsFeaturedAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}/feature")]
        public async Task<IActionResult> UnfeatureProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            await _productService.RemoveFromFeaturedAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            await _productService.UpdateStockQuantityAsync(id, request.Quantity, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/slug")]
        public async Task<IActionResult> UpdateSlug(int id, [FromBody] UpdateSlugRequest request, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new NotFoundException("Product", id);

            // Use the service method which handles slug generation and uniqueness
            await _productService.SetSlugAsync(id, request.Title, cancellationToken);
            return NoContent();
        }
    }

    public class UpdateStockRequest
    {
        public int Quantity { get; set; }
    }

    public class UpdateSlugRequest
    {
        public string Title { get; set; }
    }
}