using System;
namespace EcommerceAPI.Models.CartDTO
{
	public class CartPublicDTO
	{
        public required int ProductId { get; set; }

        //public required string UserId { get; set; }
        //public virtual required User User { get; set; }

        public int Quantity { get; set; }
    }
}

