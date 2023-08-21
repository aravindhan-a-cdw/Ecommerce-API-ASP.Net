using System;
namespace EcommerceAPI.Models.DTO.User
{
	public class UserBaseDTO
	{
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public GenderType Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; } = null;
    }
}

