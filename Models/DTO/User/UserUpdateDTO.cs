﻿using System;
namespace EcommerceAPI.Models.DTO.User
{
	public class UserUpdateDTO: UserBaseDTO
	{
        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }
    }
}

