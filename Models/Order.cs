using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
	public class Order
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string UserId { get; set; }
        public required virtual User User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

