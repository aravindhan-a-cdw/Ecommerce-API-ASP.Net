using System;
namespace EcommerceAPI.Models
{
	public class OrderItems
	{
		public required Order Order { get; set; }
		public required int OrderId { get; set; }

		public required Product Product { get; set; }
		public required int ProductId { get; set; }

		public required int Quantity { get; set; }

		public required int Price { get; set; }

	}
}

