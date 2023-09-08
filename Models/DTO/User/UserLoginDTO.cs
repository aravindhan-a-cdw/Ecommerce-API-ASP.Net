using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.UserDTO
{
	public class UserLoginDTO
	{
        [EmailAddress(ErrorMessage ="Invalid Email Address")]
        public required string Email { get; set; }

        [MinLength(8, ErrorMessage = "Minimum Password length is 8")]
        public required string Password { get; set; }
    }
}

