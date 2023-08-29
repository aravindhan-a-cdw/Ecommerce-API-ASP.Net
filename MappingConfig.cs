using AutoMapper;
using EcommerceAPI.Models.UserDTO;
using EcommerceAPI.Models.ProductDTO;
using EcommerceAPI.Models.DTO.CategoryDTO;
using EcommerceAPI.Models;
using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Models.DTO.InventoryDTO;

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

			// Cart Mapper
			CreateMap<CartItemAddDTO, Cart>();
			CreateMap<Cart, CartPublicDTO>();
			CreateMap<Cart, CartItemDTO>();

			// Inventory Mapper
			CreateMap<InventoryCreateDTO, Inventory>();
			CreateMap<Inventory, InventoryPublicDTO>();
			CreateMap<InventoryUpdateDTO, Inventory>();
		}
	}
}

