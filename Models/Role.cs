using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
	public class Role
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public required string Name { get; set; }

        [Required]
        [StringLength(1024)]
        public required string Description { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        
	}
}

