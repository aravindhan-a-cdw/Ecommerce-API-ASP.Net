using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public enum GenderType
	{
		Male, Female, Others
	}

    /*
     * @author Aravindhan A
     * @description This is the User Model which extends IdentityUser with our additional fields
     */
	public class User: IdentityUser
	{
		//[Key]
		//[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		//public override int Id { get; set; }

		[Required]
		[StringLength(64)]
		public required string FirstName { get; set; }

		[Required]
		[StringLength(64)]
		public required string LastName { get; set; }

		//[EmailAddress(ErrorMessage = "Invalid Email Address")]
		//[Required]
		//[StringLength(128)]
		//public required string Email { get; set; }

		public GenderType? Gender { get; set; } = null;

		public DateOnly? DateOfBirth { get; set; } = null;

		public ICollection<Cart> CartItems { get; set; } = new List<Cart>();

		public ICollection<Order> Orders { get; set; } = new List<Order>();
	}
}

