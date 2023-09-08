using System;
using EcommerceAPI.Models.ProductDTO;

namespace EcommerceAPI.Models.DTO.CategoryDTO
{
	public class CategoryPublicDTO: CategoryBaseDTO
	{
		public int Id { get; set; }

		public required List<ProductBaseDTO> Products { get; set; }
	}
}

