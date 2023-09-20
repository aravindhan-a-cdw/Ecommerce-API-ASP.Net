using System;
namespace EcommerceAPI.Models.CartDTO
{
	public class CartItemAddDTO
	{
        public required int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}

