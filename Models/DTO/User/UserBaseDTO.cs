using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.UserDTO
{
	public class UserBaseDTO
	{
        [StringLength(64)]
        [RegularExpression("[a-zA-Z]+", ErrorMessage ="Firstname should only contain alphabets")]
        public required string FirstName { get; set; }

        [StringLength(64)]
        [RegularExpression("[a-zA-Z]+")]
        public required string LastName { get; set; }

        [EmailAddress()]
        public required string Email { get; set; }

        public GenderType Gender { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; } = null;

        public override string ToString()
        {
            return $"User<FirstName={FirstName}, LastName={LastName}, Email={Email}, Gender={Gender}, DateOfBirth={DateOfBirth}>";
        }
    }
}

