using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Public;
using Catalog_Service.src.CrossCutting.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Public
{
    [ApiController]
    [Route("api/public/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public CategoryController(
            ICategoryService categoryService,
            IProductService productService,
            IMapper mapper)
        {
            _categoryService = categoryService;
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetActiveCategoriesAsync(cancellationToken);
            var categoryResponses = _mapper.Map<IEnumerable<CategoryResponse>>(categories);
            return Ok(categoryResponses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetCategory(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            if (category == null || !category.IsActive)
                throw new NotFoundException("Category", id);

            var categoryResponse = _mapper.Map<CategoryResponse>(category);
            return Ok(categoryResponse);
        }

        [HttpGet("{id}/subcategories")]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetSubCategories(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            if (category == null || !category.IsActive)
                throw new NotFoundException("Category", id);

            var subCategories = await _categoryService.GetSubCategoriesAsync(id, cancellationToken);
            var subCategoryResponses = _mapper.Map<IEnumerable<CategoryResponse>>(subCategories);
            return Ok(subCategoryResponses);
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetCategoryProducts(
      int id,
      [FromQuery] int count = 20,
      CancellationToken cancellationToken = default)
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            if (category == null || !category.IsActive)
                throw new NotFoundException("Category", id);

            var products = await _productService.GetByCategoryAsync(id, cancellationToken);
            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products.Take(count));
            return Ok(productResponses);
        }

        [HttpGet("root")]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetRootCategories(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetRootCategoriesAsync(cancellationToken);
            var categoryResponses = _mapper.Map<IEnumerable<CategoryResponse>>(categories);
            return Ok(categoryResponses);
        }

        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategoryTree(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetCategoryTreeAsync(cancellationToken);
            var categoryResponses = _mapper.Map<IEnumerable<CategoryResponse>>(categories);
            return Ok(categoryResponses);
        }

        [HttpGet("{id}/path")]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategoryPath(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            if (category == null || !category.IsActive)
                throw new NotFoundException("Category", id);

            var path = await _categoryService.GetCategoryPathAsync(id, cancellationToken);
            var pathResponses = _mapper.Map<IEnumerable<CategoryResponse>>(path);
            return Ok(pathResponses);
        }
    }
}
