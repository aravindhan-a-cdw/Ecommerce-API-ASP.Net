using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.UserDTO
{
	public class UserUpdateDTO: UserBaseDTO
	{
        [MinLength(8)]
        public string? CurrentPassword { get; set; }

        [MinLength(8)]
        public string? NewPassword { get; set; }

        public override string ToString()
        {
            return base.ToString() + $"With CurrentPassword={CurrentPassword}, NewPassword={NewPassword}";
        }
    }
}

