using System;
namespace EcommerceAPI.Models.DTO.User
{
	public class UserLoginDTO
	{
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}

