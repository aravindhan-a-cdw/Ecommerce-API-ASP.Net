using System;
namespace EcommerceAPI.Models
{
    /*
     * @author Aravindhan A
     * @description This is the Inventory Model which will be used in DbContext to create Table in Db
     */
    public class Inventory
	{
		public int Id { get; set; }

		public int ProductId { get; set; }

		public required Product Product { get; set; }

		public required User CreatedBy { get; set; }

		public required int QuantityAvailable { get; set; }

		public required int QuantitySold { get; set; }

		public required int Price { get; set; }
	}
}

