using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Models.DTO.OrderDTO;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Services.IServices;

namespace EcommerceAPI.Services
{
    public class CartService: ICartService
	{
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<OrderItem> _orderItemsRepository;
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public CartService(
            IRepository<Cart> cartRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
            IRepository<Inventory> inventoryRepository,
            IRepository<User> userRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _orderItemsRepository = orderItemRepository;
            _inventoryRepository = inventoryRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        async public Task<User> GetUserAsync(string userEmail)
        {
            var user = await _userRepository.GetAsync(record => record.Email == userEmail);
            if (user == null)
            {
                // Logout or invalidate the user token now
                throw new BadHttpRequestException("User doesn't exist in the database!");
            }
            return user;
        }

        async public Task<List<CartPublicDTO>> GetCartItemsAsync(string userEmail)
        {
            // Retrieve user by email
            var user = await GetUserAsync(userEmail);

            // Retrieve cart items for the user
            var cartItems = await _cartRepository.GetAllAsync(record => record.UserId == user.Id, Include: new() { "Product" });

            // Check and remove expired items
            foreach (var item in cartItems)
            {
                if (DateTime.UtcNow > (item.CreatedAt + new TimeSpan(minutes: 30, hours: 0, seconds: 0)))
                {
                    await _cartRepository.RemoveAsync(item);
                }
            }

            return _mapper.Map<List<CartPublicDTO>>(cartItems);
        }

        async public Task<CartPublicDTO> AddProductToCartAsync(string userEmail, CartItemAddDTO itemAddDTO)
        {
            var user = await GetUserAsync(userEmail);

            // Retrieve product by ID
            var product = await _productRepository.GetAsync(record => record.Id == itemAddDTO.ProductId) ?? throw new BadHttpRequestException("The product you are trying to add doesn't exist");

            var cartItem = _mapper.Map<Cart>(itemAddDTO);
            cartItem.UserId = user.Id;

            var existing = await _cartRepository.GetAsync(record => record.UserId == cartItem.UserId && record.ProductId == cartItem.ProductId, Include: new() { "Product" });

            if (existing != null)
            {
                // Since the product is already in the cart, just increase the quantity
                existing.Quantity += cartItem.Quantity;
                await _cartRepository.UpdateAsync(existing);
                return _mapper.Map<CartPublicDTO>(existing);
            }

            cartItem.CreatedAt = DateTime.UtcNow;
            var cartDb = await _cartRepository.CreateAsync(cartItem);
            return _mapper.Map<CartPublicDTO>(cartDb);
        }

        async public Task<CartPublicDTO> UpdateProductInCartAsync(string userEmail, CartItemAddDTO itemAddDTO)
        {
            var user = await GetUserAsync(userEmail);
            var cartItem = _mapper.Map<Cart>(itemAddDTO);
            cartItem.UserId = user.Id;

            var existing = await _cartRepository.GetAsync(record => record.UserId == cartItem.UserId && record.ProductId == cartItem.ProductId, NoTracking: true, Include: new() { "Product" });

            if (existing == null)
            {
                throw new BadHttpRequestException("The product is not in the cart");
            }

            existing.Quantity = itemAddDTO.Quantity;
            var cartDb = await _cartRepository.UpdateAsync(existing);
            return _mapper.Map<CartPublicDTO>(cartItem);
        }

        async public Task<bool> DeleteProductInCartAsync(string userEmail, int productId)
        {
            var user = await GetUserAsync(userEmail);
            var existing = await _cartRepository.GetAsync(record => record.UserId == user.Id && record.ProductId == productId, NoTracking: true) ?? throw new BadHttpRequestException("The product is not available in the cart");
            await _cartRepository.RemoveAsync(existing);
            return true;
        }

        async public Task<OrderPublicDTO> ProceedToCheckoutAsync(string userEmail)
        {
            var user = await GetUserAsync(userEmail);
            var cartItems = await _cartRepository.GetAllAsync(record => record.UserId == user.Id, Include: new() { "Product" });
            
            if (cartItems.Count == 0)
            {
                throw new BadHttpRequestException("Add some products to checkout");
            }
            // Check if any product is out of stock
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
                throw new BadHttpRequestException("The following product(s) are out of stock: " + String.Join(",", outOfStockProducts));
            }
            // Create order and add order items to the order and also update the inventory
            var order = new Order { UserId = user.Id };
            var orderDb = await _orderRepository.CreateAsync(order);
            try
            {
                foreach (var cartItem in cartItems)
                {
                    var inventory = await _inventoryRepository.GetAsync(record => record.ProductId == cartItem.ProductId && record.QuantityAvailable >= cartItem.Quantity);
                    if (inventory == null)
                    {
                        throw new BadHttpRequestException($"The Product {cartItem.Product.Name} is out of stock!");
                    }
                    var orderItem = new OrderItem { OrderId = orderDb.Id, ProductId = cartItem.ProductId, Quantity = cartItem.Quantity, Price = inventory.Price };
                    inventory.QuantitySold = inventory.QuantitySold + cartItem.Quantity;
                    inventory.QuantityAvailable = inventory.QuantityAvailable - cartItem.Quantity;
                    await _orderItemsRepository.CreateAsync(orderItem);
                    await _inventoryRepository.UpdateAsync(inventory);
                    await _cartRepository.RemoveAsync(cartItem);
                    // This will lead to inconsistency as when some item in middle goes out of stock then other products also suffer
                }
            }
            catch (BadHttpRequestException)
            {
                // As BadHttpRequestException occured delete created order
                if (orderDb != null)
                {
                    await _orderRepository.RemoveAsync(orderDb);
                }
                throw;
            }
            return _mapper.Map<OrderPublicDTO>(orderDb);
        }
    }

}

