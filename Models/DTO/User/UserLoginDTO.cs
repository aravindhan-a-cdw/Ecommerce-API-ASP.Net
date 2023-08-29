using System;
namespace EcommerceAPI.Models.UserDTO
{
	public class UserLoginDTO
	{
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}

