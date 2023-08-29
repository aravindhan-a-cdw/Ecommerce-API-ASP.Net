using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "customer")]
    public class CartController : ControllerBase
    {
        private readonly Repository<Cart> _cartRepository;
        private readonly Repository<User> _userRepository;
        private readonly IMapper _mapper;

        public CartController(Repository<Cart> cartRepository, Repository<User> userRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        async public Task<IActionResult> GetCart()
        {
            
            var User = HttpContext.User;

            var user = await _userRepository.GetAsync(record => record.Email == User.Identity.Name);
            var cartItems = await _cartRepository.GetAllAsync(record => record.UserId == user.Id);
            
            return Ok(_mapper.Map<List<CartPublicDTO>>(cartItems));
        }

        [HttpPost]
        async public Task<IActionResult> AddProductToCart([FromBody] CartItemAddDTO itemAddDTO)
        {
            var User = HttpContext.User;
            
            var user = await _userRepository.GetAsync(record => record.Email == User.Identity.Name);
            var cartItem = _mapper.Map<Cart>(itemAddDTO);
            cartItem.UserId = user.Id;
            var existing = await _cartRepository.GetAsync(record => record.UserId == cartItem.UserId && record.ProductId == cartItem.ProductId);
            if(existing != null)
            {
                existing.Quantity += cartItem.Quantity;
                await _cartRepository.UpdateAsync(existing);
                return Ok(existing);
            }
            cartItem.CreatedAt = DateTime.UtcNow;
            var cartDb = await _cartRepository.CreateAsync(cartItem);
            return Ok(cartDb);
        }

        [HttpPut]
        async public Task<IActionResult> UpdateProductInCart([FromBody] CartItemAddDTO itemAddDTO)
        {
            var User = HttpContext.User;
            
            var user = await _userRepository.GetAsync(record => record.Email == User.Identity.Name, NoTracking: true);
            var cartItem = _mapper.Map<Cart>(itemAddDTO);
            cartItem.UserId = user.Id;
            var existing = await _cartRepository.GetAsync(record => record.UserId == cartItem.UserId && record.ProductId == cartItem.ProductId, NoTracking: true);
            if (existing == null)
            {
                return BadRequest("The product is not in cart");
            }
            existing.Quantity = itemAddDTO.Quantity;
            var cartDb = await _cartRepository.UpdateAsync(existing);
            return Ok(_mapper.Map<CartPublicDTO>(cartItem));
        }

        [HttpDelete("{productId}")]
        async public Task<IActionResult> DeleteProductInCart([FromRoute] int productId)
        {
            var User = HttpContext.User;

            var user = await _userRepository.GetAsync(record => record.Email == User.Identity.Name, NoTracking: true);
            
            var existing = await _cartRepository.GetAsync(record => record.UserId == user.Id && record.ProductId == productId, NoTracking: true);
            if (existing == null)
            {
                return BadRequest("The product is not in cart");
            }
            await _cartRepository.RemoveAsync(existing);
            return NoContent();
        }

        // TODO: Complete Checkout
        [HttpPost("checkout")]
        async public Task<IActionResult> ProceedToCheckout()
        {
            return Ok();
        }

    }
}

