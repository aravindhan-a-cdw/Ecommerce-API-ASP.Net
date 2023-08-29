using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.UserDTO
{
	public class UserCreateDTO: UserBaseDTO
	{
        public required string Password { get; set; }

        public override string ToString()
        {
            return base.ToString() + $"With Password {Password}";
        }
    }
}

