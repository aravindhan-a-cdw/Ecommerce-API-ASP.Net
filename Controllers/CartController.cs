using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Services.IServices;
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
    [Route("[controller]")]
    [Authorize(Roles = "customer")]
    public class CartController : ControllerBase
    {
        /// <summary>
        /// Controller Class for Cart related routes
        /// </summary>

        private readonly ICartService _cartService;
        private string _email = "";

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
            //if(HttpContext.User.Identity != null)
            //_email = HttpContext.User.Identity.Name ?? "";
        }


        [HttpGet]
        [SwaggerOperation(summary: "Get Products in Cart", description: "This endpoint allows Customer to get Products in the cart")]
        async public Task<IActionResult> GetCart()
        {
            /// <summary>
            /// Route to Get Cart items of a user
            /// </summary>
            var userEmail = HttpContext.User.Identity.Name ?? "";
            var cartItems = await _cartService.GetCartItemsAsync(userEmail);
            return Ok(cartItems);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Add Product to cart", description: "This endpoint allows Customer to add Product to cart or increase the quantity of the Product in the cart")]
        [HttpPost]
        async public Task<IActionResult> AddProductToCart([FromBody] CartItemAddDTO itemAddDTO)
        {
            /// <summary>
            /// Route to Add a new product to user cart or update the cart by adding the quantity
            /// </summary>

            var userEmail = HttpContext.User.Identity.Name ?? "";

            var cart = await _cartService.AddProductToCartAsync(userEmail, itemAddDTO);
            return Ok(cart);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Update the Products in the cart", description: "This endpoint allows Customer to Update the quantity in the Cart")]
        [HttpPut]
        async public Task<IActionResult> UpdateProductInCart([FromBody] CartItemAddDTO itemAddDTO)
        {
            /// <summary>
            /// Route to Update Cart quantity
            /// </summary>

            var userEmail = HttpContext.User.Identity.Name ?? "";

            var cart = await _cartService.UpdateProductInCartAsync(userEmail, itemAddDTO);
            return Ok(cart);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Delete a Product from cart", description: "This endpoint allows Customer to delete a Product from Cart")]
        [HttpDelete("{productId}")]
        async public Task<IActionResult> DeleteProductInCart([FromRoute] int productId)
        {
            /// <summary>
            /// Route to delete product from cart
            /// </summary>

            var userEmail = HttpContext.User.Identity.Name ?? "";

            var cart = await _cartService.DeleteProductInCartAsync(userEmail, productId);
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Proceed to checkout with the products in the Cart", description: "This endpoint allows Customer to create Order with the products in the Cart")]
        [HttpPost("checkout")]
        async public Task<IActionResult> ProceedToCheckout()
        {
            /// <summary>
            /// Route to checkout products in cart by creating an order
            /// </summary>

            var userEmail = HttpContext.User.Identity.Name ?? "";

            var order = await _cartService.ProceedToCheckoutAsync(userEmail);
            return Ok(order);

        }
    }
}

