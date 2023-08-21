using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTO.User
{
	public class UserBaseDTO
	{
        [StringLength(64)]
        public required string FirstName { get; set; }

        [StringLength(64)]
        public required string LastName { get; set; }

        [StringLength(128)]
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

