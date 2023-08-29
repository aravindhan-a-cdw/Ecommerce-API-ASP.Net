using System;
using EcommerceAPI.Models.CartDTO;

namespace EcommerceAPI.Models.UserDTO
{
	public class UserPublicDTO: UserBaseDTO
	{
        public required string id { get; set; }

        public required ICollection<CartPublicDTO> CartItems { get; set; }

        public required ICollection<Order> Orders { get; set; }
    }
}

