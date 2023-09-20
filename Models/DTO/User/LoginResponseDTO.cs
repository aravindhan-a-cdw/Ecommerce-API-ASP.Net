using System;

namespace EcommerceAPI.Models.UserDTO
{
	public class LoginResponseDTO
	{
		public required UserPublicDTO user { get; set; }
		public required string token { get; set; }
	}
}

