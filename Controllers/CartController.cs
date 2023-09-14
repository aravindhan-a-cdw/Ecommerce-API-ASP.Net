using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{

    /*
     * @author Aravindhan A
     * @description This is the controller class for Cart related Routes. It also contains route to Proceed the items in the cart to checkout
     */

    [ApiController]
    [Route(Constants.Routes.CONTROLLER)]
    [Authorize(Roles = Constants.Roles.CUSTOMER)]
    public class CartController : ControllerBase
    {
        /// <summary>
        /// Controller Class for Cart related routes
        /// </summary>

        private readonly ICartService _cartService;
        private string _email = "";

        public CartController(ICartService cartService, IHttpContextAccessor context)
        {
            _cartService = cartService;
            if(context.HttpContext.User.Identity != null)
            {
                _email = context.HttpContext.User.Identity.Name ?? "";
            }
        }


        [HttpGet]
        [SwaggerOperation(summary: Constants.Swagger.Cart.GET_CART_SUMMARY, description: Constants.Swagger.Cart.GET_CART_DESCRIPTION)]
        async public Task<IActionResult> GetCart()
        {
            /// <summary>
            /// Route to Get Cart items of a user
            /// </summary>
            
            var cartItems = await _cartService.GetCartItemsAsync(_email);
            return Ok(cartItems);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Cart.ADD_PRODUCT_TO_CART_SUMMARY, description: Constants.Swagger.Cart.ADD_PRODUCT_TO_CART_DESCRIPTION)]
        [HttpPost]
        async public Task<IActionResult> AddProductToCart([FromBody] CartItemAddDTO itemAddDTO)
        {
            /// <summary>
            /// Route to Add a new product to user cart or update the cart by adding the quantity
            /// </summary>

            var cart = await _cartService.AddProductToCartAsync(_email, itemAddDTO);
            return Ok(cart);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Cart.UPDATE_PRODUCT_IN_CART_SUMMARY, description: Constants.Swagger.Cart.UPDATE_PRODUCT_IN_CART_DESCRIPTION)]
        [HttpPut]
        async public Task<IActionResult> UpdateProductInCart([FromBody] CartItemAddDTO itemAddDTO)
        {
            /// <summary>
            /// Route to Update Cart quantity
            /// </summary>

            var cart = await _cartService.UpdateProductInCartAsync(_email, itemAddDTO);
            return Ok(cart);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Cart.DELETE_PRODUCT_IN_CART_SUMMARY, description: Constants.Swagger.Cart.DELETE_PRODUCT_IN_CART_DESCRIPTION)]
        [HttpDelete("{productId}")]
        async public Task<IActionResult> DeleteProductInCart([FromRoute] int productId)
        {
            /// <summary>
            /// Route to delete product from cart
            /// </summary>

            await _cartService.DeleteProductInCartAsync(_email, productId);
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Cart.PROCEED_TO_CHECKOUT_SUMMARY, description: Constants.Swagger.Cart.PROCEED_TO_CHECKOUT_DESCRIPTION)]
        [HttpPost("checkout")]
        async public Task<IActionResult> ProceedToCheckout()
        {
            /// <summary>
            /// Route to checkout products in cart by creating an order
            /// </summary>

            var order = await _cartService.ProceedToCheckoutAsync(_email);
            return Ok(order);

        }
    }
}

