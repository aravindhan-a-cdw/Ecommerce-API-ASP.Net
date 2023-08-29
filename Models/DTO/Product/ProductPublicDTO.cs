using System;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceAPI.Models.DTO.CategoryDTO;

namespace EcommerceAPI.Models.ProductDTO
{
	public class ProductPublicDTO: ProductBaseDTO
	{
		public required int id { get; set; }
		public required CategoryPublicDTO Category { get; set; }
	}
}

