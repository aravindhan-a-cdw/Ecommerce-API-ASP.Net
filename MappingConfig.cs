using AutoMapper;
using EcommerceAPI.Models.UserDTO;
using EcommerceAPI.Models.ProductDTO;
using EcommerceAPI.Models.DTO.CategoryDTO;
using EcommerceAPI.Models;
using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Models.DTO.InventoryDTO;
using EcommerceAPI.Models.DTO.OrderDTO;

namespace EcommerceAPI
{
	public class MappingConfig: Profile
	{
		public MappingConfig()
		{
			// User Mapper

			CreateMap<UserCreateDTO, User>();
			CreateMap<User, UserPublicDTO>();

			// Product Mapper
			CreateMap<ProductCreateDTO, Product>();
			CreateMap<Product, ProductPublicDTO>();
			CreateMap<ProductUpdateDTO, Product>();

			// Category Mapper
			CreateMap<CategoryCreateDTO, Category>();
			CreateMap<Category, CategoryPublicDTO>();
			CreateMap<CategoryUpdateDTO, Category>();

			// Inventory Mapper
			CreateMap<InventoryCreateDTO, Inventory>();
			CreateMap<Inventory, InventoryPublicDTO>();
			CreateMap<InventoryUpdateDTO, Inventory>();

			// Cart Mapper
			CreateMap<CartItemAddDTO, Cart>();
			CreateMap<Cart, CartPublicDTO>();
			CreateMap<Cart, CartItemDTO>();

			// Order Mapper
			CreateMap<Order, OrderPublicDTO>();
			CreateMap<OrderItem, OrderItemPublicDTO>();

		}
	}
}

