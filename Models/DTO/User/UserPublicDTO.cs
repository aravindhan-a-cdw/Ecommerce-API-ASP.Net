using System;
using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Models.DTO.OrderDTO;

namespace EcommerceAPI.Models.UserDTO
{
	public class UserPublicDTO: UserBaseDTO
	{
        public required string id { get; set; }

        public required ICollection<CartPublicDTO> CartItems { get; set; }

        public required ICollection<OrderPublicDTO> Orders { get; set; }
    }
}

