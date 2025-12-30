using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Public;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Public
{
    [ApiController]
    [Route("api/public/brands")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public BrandController(
            IBrandService brandService,
            IProductService productService,
            IMapper mapper)
        {
            _brandService = brandService;
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandResponse>>> GetBrands(CancellationToken cancellationToken)
        {
            var brands = await _brandService.GetActiveBrandsAsync(cancellationToken);
            var brandResponses = _mapper.Map<IEnumerable<BrandResponse>>(brands);
            return Ok(brandResponses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandResponse>> GetBrand(int id, CancellationToken cancellationToken)
        {
            var brand = await _brandService.GetByIdAsync(id, cancellationToken);

            if (brand == null)
            {
                return NotFound(new { message = $"Brand with ID {id} not found." });
            }

            var brandResponse = _mapper.Map<BrandResponse>(brand);
            return Ok(brandResponse);
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetBrandProducts(
            int id,
            [FromQuery] int count,
            CancellationToken cancellationToken)
        {
            // ابتدا بررسی می‌کنیم که آیا برند وجود دارد یا خیر
            var brandExists = await _brandService.ExistsAsync(id, cancellationToken);
            if (!brandExists)
            {
                return NotFound(new { message = $"Brand with ID {id} not found." });
            }

            var products = await _productService.GetByBrandAsync(id, cancellationToken);
            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products.Take(count));
            return Ok(productResponses);
        }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<BrandResponse>>> GetTopBrands(
            [FromQuery] int count,
            CancellationToken cancellationToken)
        {
            var brands = await _brandService.GetTopBrandsByProductsCountAsync(count, cancellationToken);
            var brandResponses = _mapper.Map<IEnumerable<BrandResponse>>(brands);
            return Ok(brandResponses);
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<BrandResponse>>> GetFeaturedBrands(
            [FromQuery] int count,
            CancellationToken cancellationToken)
        {
            var brands = await _brandService.GetTopBrandsByRatingAsync(count, cancellationToken);
            var brandResponses = _mapper.Map<IEnumerable<BrandResponse>>(brands);
            return Ok(brandResponses);
        }
    }
}