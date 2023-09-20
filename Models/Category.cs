using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    /*
     * @author Aravindhan A
     * @description This is the Category Model which will be used in DbContext to create Table in Db
     */
    public class Category
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public required string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

