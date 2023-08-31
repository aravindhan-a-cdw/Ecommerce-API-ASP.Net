using System;
namespace EcommerceAPI.Models
{
	public class OrderItem
	{
		public Order Order { get; set; }
		public required int OrderId { get; set; }

		public Product Product { get; set; }
		public required int ProductId { get; set; }

		public required int Quantity { get; set; }

		public required int Price { get; set; }

	}
}

