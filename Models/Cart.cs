using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
	public class Cart
	{
        public int ProductId { get; set; }
        public virtual required Product Product { get; set; }

        public required string UserId { get; set; }
        public virtual required User User { get; set; }

        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

