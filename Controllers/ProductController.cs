﻿using EcommerceAPI.Models.ProductDTO;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Utilities;
using EcommerceAPI.Utilities.IUtilities;
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
        private readonly IRequestUtility _requestsUtility;

        public ProductController(IProductService productService, IHttpContextAccessor context, ICustomerUtility customerUtility, IRequestUtility requestsUtility)
        {
            _productService = productService;
            _requestsUtility = requestsUtility;
            if (context.HttpContext.User.Identity != null)
            {
                var _email = context.HttpContext.User.Identity.Name ?? "";
                customerUtility.UpdateLastAccess(_email);
            }
        }


        [HttpGet(Name = Constants.Routes.Product.GET_ALL_PRODUCTS)]
        [AllowAnonymous]
        [ResponseCache(Duration = 60)]
        [SwaggerOperation(summary: Constants.Swagger.Product.GET_ALL_PRODUCTS_SUMMARY, description: Constants.Swagger.Product.GET_ALL_PRODUCTS_DESCRIPTION)]
        async public Task<IActionResult> GetAllProducts()
        {
            var data = await _requestsUtility.GetBannedProducts();
            var bannedProducts = data.FindAll(p => p.isBanned == true).Select(p => p.productId);
            var trialProducts = data.FindAll(p => p.isTrialGoing == true).Select(p => p.productId);
            var products = await _productService.GetAllProductsAsync(record => !bannedProducts.Contains(record.Name));
            foreach(var product in products)
            {
                if(trialProducts.Contains(product.Name))
                {
                    product.HasDisclaimer = true;
                }
            }
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
            var data = await _requestsUtility.GetBannedProducts();
            var hasBeenBanned = false;
            var isInTrial = false;
            foreach (var bannedProduct in data)
            {
                if (product.Name == bannedProduct.productId)
                {
                    hasBeenBanned = bannedProduct.isBanned;
                    isInTrial = bannedProduct.isTrialGoing;
                }
            }
            if (hasBeenBanned)
            {
                throw new BadHttpRequestException(Constants.Messages.BANNED_PRODUCT, StatusCodes.Status400BadRequest);
            }
            product.HasDisclaimer = isInTrial;
            return Ok(product);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Product.CREATE_PRODUCT_SUMMARY, description: Constants.Swagger.Product.CREATE_PRODUCT_DESCRIPTION)]
        [HttpPost]
        async public Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productDto)
        {
            var data = await _requestsUtility.GetBannedProducts();
            var hasBeenBanned = false;
            var isInTrial = false;
            foreach (var bannedProduct in data)
            {
                if(productDto.Name == bannedProduct.productId)
                {
                    hasBeenBanned = bannedProduct.isBanned;
                    isInTrial = bannedProduct.isTrialGoing;
                }
            }
            if (hasBeenBanned)
            {
                throw new BadHttpRequestException(Constants.Messages.BANNED_PRODUCT, StatusCodes.Status400BadRequest);
            }
            var product = await _productService.CreateProductAsync(productDto);
            product.HasDisclaimer = isInTrial;
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

