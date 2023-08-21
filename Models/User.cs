using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
	public enum GenderType
	{
		Male, Female, Others
	}

	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(64)]
		public required string FirstName { get; set; }

		[Required]
		[StringLength(64)]
		public required string LastName { get; set; }

		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[Required]
		[StringLength(128)]
		public required string Email { get; set; }

		public GenderType? Gender { get; set; } = null;

		public DateOnly? DateOfBirth { get; set; } = null;

		[StringLength(32)]
		public required string Password { get; set; }

		public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

		//public User()
		//{
		//	this.Roles.Ad
		//}
	}
}

