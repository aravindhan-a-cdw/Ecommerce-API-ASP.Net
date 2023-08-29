﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
	public class Product
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(96)]
		public required string Name { get; set; }

		[ForeignKey("Category")]
		public required int CategoryId { get; set; }
		public required virtual Category Category { get; set; }

		[Required]
		[StringLength(1024)]
		public required string Description { get; set; }

		[Required]
		public required List<string> Images { get; set; }
		
	}
}

