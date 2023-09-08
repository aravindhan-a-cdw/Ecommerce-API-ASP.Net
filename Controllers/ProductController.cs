
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models.ProductDTO;
using EcommerceAPI.Repository;
using EcommerceAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Swashbuckle.AspNetCore.Annotations;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Services.IServices;

namespace EcommerceAPI.Controllers
{
    /*
     * @author Aravindhan A
     * @description This is the controller class for Product related Routes. Admin can CRUD a product whereas user can only View or Read product.
     */

    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        [ResponseCache(Duration=60)]
        [SwaggerOperation(summary: "Get all Products", description: "This endpoint gets list of all products")]
        async public Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: "Get details of a Single Product", description: "This endpoint gets a product with its id")]
        [HttpGet("{productId}", Name = "GetProduct")]
        async public Task<IActionResult> GetProduct([FromRoute] int productId)
        {
            var product = await _productService.GetProductAsync(productId);
            return Ok(product);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Create a new product", description: "This endpoint allows admin to create a new product")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        async public Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productDto)
        {
            var product = await _productService.CreateProductAsync(productDto);
            return Ok(product);
        }

        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: "Update Product details", description: "This endpoint allows admin to update a product")]
        [Authorize(Roles = "admin")]
        [HttpPut("{productId}")]
        async public Task<IActionResult> UpdateProduct([FromRoute] int productId, [FromBody] ProductUpdateDTO productUpdate)
        {
            var updated = await _productService.UpdateProductAsync(productId, productUpdate);
            return Ok(updated);
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: "Delete Product", description: "This endpoint allows admin to delete a product")]
        [HttpDelete("{productId}")]
        [Authorize(Roles = "admin")]
        async public Task<IActionResult> DeleteProduct([FromRoute] int productId)
        {
            await _productService.DeleteProductAsync(productId);
            return NoContent();
        }

    }
}

