using System;
using EcommerceAPI.Models.ProductDTO;

namespace EcommerceAPI.Models.DTO.OrderDTO
{
	public class OrderItemPublicDTO
	{
		public int OrderId { get; set; }

		public ProductPublicDTO Product { get; set; }

		public int Quantity { get; set; }

		public int Price { get; set; }
	}
}

