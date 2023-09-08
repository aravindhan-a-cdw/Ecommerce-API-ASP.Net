using System;
namespace EcommerceAPI.Models
{
    /*
     * @author Aravindhan A
     * @description This is the OrderItem Model which will be used in DbContext to create Table in Db
     */
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

