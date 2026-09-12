using Microsoft.AspNetCore.Mvc;
using PetShop.DTOs;
using PetShop.Services;

namespace PetShop.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Product name is required.");
            }
            if (dto.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            // Category validation
            var category = await _categoryService.GetCategoryByIdAsync(dto.ProductCategoryId);
            if (category == null)
            {
                return BadRequest($"Category with ID {dto.ProductCategoryId} does not exist.");
            }

            var createdProduct = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Product name is required.");
            }
            if (dto.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            // Category validation
            var category = await _categoryService.GetCategoryByIdAsync(dto.ProductCategoryId);
            if (category == null)
            {
                return BadRequest($"Category with ID {dto.ProductCategoryId} does not exist.");
            }

            var updated = await _productService.UpdateProductAsync(id, dto);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
