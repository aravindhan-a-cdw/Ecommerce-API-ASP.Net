using System;
namespace EcommerceAPI.Models.CartDTO
{
	public class CartItemDTO
	{
        public virtual required Product Product { get; set; }

        //public required string UserId { get; set; }
        //public virtual required User User { get; set; }

        public int Quantity { get; set; }
        //public DateTime CreatedAt { get; set; }
    }
}

