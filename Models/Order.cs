using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    /*
     * @author Aravindhan A
     * @description This is the Order Model which will be used in DbContext to create Table in Db
     */
    public class Order
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

