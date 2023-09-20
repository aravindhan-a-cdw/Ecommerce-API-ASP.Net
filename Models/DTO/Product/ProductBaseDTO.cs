using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.ProductDTO
{
	public class ProductBaseDTO
	{

        [Required]
        [StringLength(96)]
        //[RegularExpression(@"^[A-Z0-9]+$")]
        public required string Name { get; set; }

        [Required]
        [StringLength(1024)]
        public required string Description { get; set; }

        [Required]
        public required List<string> Images { get; set; }

        public required int CategoryId { get; set; }
    }
}

