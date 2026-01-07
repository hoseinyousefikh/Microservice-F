using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._03_Endpoints.DTOs.Requests.Admin;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin;
using Catalog_Service.src.CrossCutting.Security;
using Catalog_Service.src.CrossCutting.Validation.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public class AdminCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminCategoryController> _logger;

        public AdminCategoryController(
            ICategoryService categoryService,
            IMapper mapper,
            ILogger<AdminCategoryController> logger)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            var response = _mapper.Map<IEnumerable<AdminCategoryResponse>>(categories);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<AdminCategoryResponse>(category);
            return Ok(response);
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetCategoryTree()
        {
            var categories = await _categoryService.GetCategoryTreeAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var validator = new CreateCategoryValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Try to get the user ID from standard 'NameIdentifier' claim first, then fallback to 'sub'
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var category = await _categoryService.CreateAsync(
                request.Name,
                request.Description,
                request.DisplayOrder,
                userId, // Pass the extracted user ID
                request.ParentCategoryId,
                request.ImageUrl,
                request.MetaTitle,
                request.MetaDescription);

            var response = _mapper.Map<AdminCategoryResponse>(category);
            return CreatedAtAction(nameof(GetCategory), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            var validator = new UpdateCategoryValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _categoryService.UpdateAsync(
                id,
                request.Name,
                request.Description,
                request.DisplayOrder,
                request.ImageUrl,
                request.MetaTitle,
                request.MetaDescription);

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateCategory(int id)
        {
            await _categoryService.ActivateAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            await _categoryService.DeactivateAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/move")]
        public async Task<IActionResult> MoveCategory(int id, [FromBody] MoveCategoryRequest request)
        {
            await _categoryService.MoveCategoryAsync(id, request.NewParentId);
            return NoContent();
        }
    }

    public class MoveCategoryRequest
    {
        public int? NewParentId { get; set; }
    }
}