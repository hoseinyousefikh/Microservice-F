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
    public class AdminBrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminBrandController> _logger;

        public AdminBrandController(
            IBrandService brandService,
            IMapper mapper,
            ILogger<AdminBrandController> logger)
        {
            _brandService = brandService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _brandService.GetAllAsync();
            var response = _mapper.Map<IEnumerable<AdminBrandResponse>>(brands);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<AdminBrandResponse>(brand);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandRequest request)
        {
            var validator = new CreateBrandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // --- DEBUGGING CODE ---
            // Print all claims to the console/log to find the correct one
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User is authenticated. Claims:");
                foreach (var claim in User.Claims)
                {
                    _logger.LogInformation($"Type: {claim.Type}, Value: {claim.Value}");
                }
            }
            else
            {
                _logger.LogWarning("User is NOT authenticated.");
            }
            // --- END DEBUGGING CODE ---

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // You can also check for other common claim types like "nameidentifier" or "userid"
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }
            }

            var brand = await _brandService.CreateAsync(
                request.Name,
                request.Description,
                userId, // Pass the extracted user ID
                request.LogoUrl,
                request.WebsiteUrl,
                request.MetaTitle,
                request.MetaDescription);

            var response = _mapper.Map<AdminBrandResponse>(brand);
            return CreatedAtAction(nameof(GetBrand), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] UpdateBrandRequest request)
        {
            var validator = new UpdateBrandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _brandService.UpdateAsync(
                id,
                request.Name,
                request.Description,
                request.LogoUrl,
                request.MetaTitle,
                request.MetaDescription);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _brandService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateBrand(int id)
        {
            await _brandService.ActivateAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateBrand(int id)
        {
            await _brandService.DeactivateAsync(id);
            return NoContent();
        }
    }
}