using System;
using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Models.DTO.OrderDTO;

namespace EcommerceAPI.Models.UserDTO
{
	public class UserPublicDTO: UserBaseDTO
	{
        public required string id { get; set; }

        public ICollection<CartPublicDTO> CartItems { get; set; } = new List<CartPublicDTO>();

        public ICollection<OrderPublicDTO> Orders { get; set; } = new List<OrderPublicDTO>();
    }
}

