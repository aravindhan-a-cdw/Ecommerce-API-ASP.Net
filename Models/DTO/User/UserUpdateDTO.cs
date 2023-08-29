﻿using System;
namespace EcommerceAPI.Models.UserDTO
{
	public class UserUpdateDTO: UserBaseDTO
	{
        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public override string ToString()
        {
            return base.ToString() + $"With CurrentPassword={CurrentPassword}, NewPassword={NewPassword}";
        }
    }
}

