using AutoMapper;
using EcommerceAPI.Models.DTO.User;
using EcommerceAPI.Models;

namespace EcommerceAPI
{
	public class MappingConfig: Profile
	{
		public MappingConfig()
		{
			CreateMap<UserCreateDTO, User>();
			CreateMap<User, UserPublicDTO>();
		}
	}
}

