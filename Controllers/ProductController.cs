
using EcommerceAPI.Models.ProductDTO;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EcommerceAPI.Controllers
{
    /*
     * @author Aravindhan A
     * @description This is the controller class for Product related Routes. Admin can CRUD a product whereas user can only View or Read product.
     */

    [ApiController]
    [Route(Constants.Routes.CONTROLLER)]
    [Authorize(Roles = Constants.Roles.ADMIN)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet(Name = Constants.Routes.Product.GET_ALL_PRODUCTS)]
        [AllowAnonymous]
        [SwaggerOperation(summary: Constants.Swagger.Product.GET_ALL_PRODUCTS_SUMMARY, description: Constants.Swagger.Product.GET_ALL_PRODUCTS_DESCRIPTION)]
        async public Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: Constants.Swagger.Product.GET_PRODUCT_SUMMARY, description: Constants.Swagger.Product.GET_PRODUCT_DESCRIPTION)]
        [AllowAnonymous]
        [HttpGet("{productId}", Name = Constants.Routes.Product.GET_PRODUCT)]
        [ResponseCache(Duration = 60 * 60)]
        async public Task<IActionResult> GetProduct([FromRoute] int productId)
        {
            var product = await _productService.GetProductAsync(productId);
            return Ok(product);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Product.CREATE_PRODUCT_SUMMARY, description: Constants.Swagger.Product.CREATE_PRODUCT_DESCRIPTION)]
        [HttpPost]
        async public Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productDto)
        {
            var product = await _productService.CreateProductAsync(productDto);
            return Ok(product);
        }

        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: Constants.Swagger.Product.UPDATE_PRODUCT_SUMMARY, description: Constants.Swagger.Product.UPDATE_PRODUCT_DESCRIPTION)]
        [HttpPut("{productId}")]
        async public Task<IActionResult> UpdateProduct([FromRoute] int productId, [FromBody] ProductUpdateDTO productUpdate)
        {
            var updated = await _productService.UpdateProductAsync(productId, productUpdate);
            return Ok(updated);
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: Constants.Swagger.Product.DELETE_PRODUCT_SUMMARY, description: Constants.Swagger.Product.DELETE_PRODUCT_DESCRIPTION)]
        [HttpDelete("{productId}")]
        async public Task<IActionResult> DeleteProduct([FromRoute] int productId)
        {
            await _productService.DeleteProductAsync(productId);
            return NoContent();
        }

    }
}

