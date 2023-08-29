using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.DTO.CategoryDTO
{
	public class CategoryBaseDTO
	{
        [Required]
        [StringLength(64)]
        public required string Name { get; set; }
	}
}

