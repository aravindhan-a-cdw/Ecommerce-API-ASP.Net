using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTO.User
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

