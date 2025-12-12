using AutoMapper;
using Catalog_Service.src._01_Domain.Core.Contracts.Services;
using Catalog_Service.src._03_Endpoints.DTOs.Responses.Admin;
using Catalog_Service.src.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.src._03_Endpoints.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Policy = AuthorizationPolicies.AdminPolicy)]
    public class AdminProductAttributeController : ControllerBase
    {
        private readonly IProductAttributeService _productAttributeService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminProductAttributeController> _logger;

        public AdminProductAttributeController(
            IProductAttributeService productAttributeService,
            IMapper mapper,
            ILogger<AdminProductAttributeController> logger)
        {
            _productAttributeService = productAttributeService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductAttributes(int productId)
        {
            var attributes = await _productAttributeService.GetByProductIdAsync(productId);
            var response = _mapper.Map<IEnumerable<ProductAttributeResponse>>(attributes);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAttribute(int id)
        {
            var attribute = await _productAttributeService.GetByIdAsync(id);
            if (attribute == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<ProductAttributeResponse>(attribute);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAttribute([FromBody] CreateProductAttributeRequest request)
        {
            var attribute = await _productAttributeService.CreateAsync(
                request.ProductId,
                request.Name,
                request.Value);

            var response = _mapper.Map<ProductAttributeResponse>(attribute);
            return CreatedAtAction(nameof(GetProductAttribute), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAttribute(int id, [FromBody] UpdateProductAttributeRequest request)
        {
            await _productAttributeService.UpdateAsync(id, request.Name, request.Value);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAttribute(int id)
        {
            await _productAttributeService.DeleteAsync(id);
            return NoContent();
        }
    }

    public class CreateProductAttributeRequest
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class UpdateProductAttributeRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
