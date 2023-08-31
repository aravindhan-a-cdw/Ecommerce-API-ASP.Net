using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Models.DTO.OrderDTO;
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
        private readonly Repository<OrderItem> _orderItemsRepository;
        private readonly InventoryRepository _inventoryRepository;
        private readonly Repository<Order> _orderRepository;
        private readonly Repository<Product> _productRepository;
        private readonly IMapper _mapper;

        public CartController(Repository<Cart> cartRepository,
            Repository<Order> orderRepository,
            Repository<OrderItem> orderItemRepository,
            Repository<Product> productRepository,
            InventoryRepository inventoryRepository,
            Repository<User> userRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _orderItemsRepository = orderItemRepository;
            _inventoryRepository = inventoryRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
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
            var product = await _productRepository.GetAsync(record => record.Id == itemAddDTO.ProductId);
            if(product == null)
            {
                return BadRequest("The product you are trying to add doesn't exist");
            }
            var cartItem = _mapper.Map<Cart>(itemAddDTO);
            cartItem.UserId = user.Id;
            var existing = await _cartRepository.GetAsync(record => record.UserId == cartItem.UserId && record.ProductId == cartItem.ProductId);
            if(existing != null)
            {
                existing.Quantity += cartItem.Quantity;
                await _cartRepository.UpdateAsync(existing);
                return Ok(_mapper.Map<CartPublicDTO>(existing));
            }
            cartItem.CreatedAt = DateTime.UtcNow;
            var cartDb = await _cartRepository.CreateAsync(cartItem);
            return Ok(_mapper.Map<CartPublicDTO>(cartDb));
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
            var User = HttpContext.User;
            var user = await _userRepository.GetAsync(record => record.Email == User.Identity.Name, NoTracking: true);
            if(user == null)
            {
                return BadRequest("User is not present in the database");
            }
            var cartItems = await _cartRepository.GetAllAsync(record => record.UserId == user.Id);
            if(cartItems.Count == 0)
            {
                return BadRequest("Add some products to checkout");
            }
            var outOfStockProducts = new List<string>();
            foreach (var cartItem in cartItems)
            {
                var inventory = await _inventoryRepository.GetAsync(record => record.ProductId == cartItem.ProductId && record.QuantityAvailable >= cartItem.Quantity);
                if (inventory == null)
                {
                    outOfStockProducts.Add(cartItem.Product.Name);
                }
            }
            if (outOfStockProducts.Count != 0)
            {
                
                return BadRequest("The following product(s) are out of stock:\n" + String.Join("\n", outOfStockProducts));
            }
            var order = new Order{UserId = user.Id};
            var orderDb = await _orderRepository.CreateAsync(order);
            try
            {
                foreach (var cartItem in cartItems)
                {
                    var inventory = await _inventoryRepository.GetAsync(record => record.ProductId == cartItem.ProductId && record.QuantityAvailable >= cartItem.Quantity);
                    if(inventory == null)
                    {
                        throw new Exception($"The Product {cartItem.Product.Name} is out of stock!");
                    }
                    var orderItem = new OrderItem { OrderId = orderDb.Id, ProductId = cartItem.ProductId, Quantity = cartItem.Quantity, Price = inventory.Price };
                    inventory.QuantitySold += cartItem.Quantity;
                    inventory.QuantityAvailable -= cartItem.Quantity;
                    await _orderItemsRepository.CreateAsync(orderItem);
                    await _inventoryRepository.UpdateAsync(inventory);
                    await _cartRepository.RemoveAsync(cartItem);
                    // This will lead to inconsistency as when some item in middle goes out of stock then other products also suffer
                }
            } catch (Exception exc)
            {
                if(orderDb != null)
                {
                    await _orderRepository.RemoveAsync(orderDb);
                }
                return BadRequest(exc.Message);
            }
            return Ok(_mapper.Map<OrderPublicDTO>(orderDb));
        }

    }
}

