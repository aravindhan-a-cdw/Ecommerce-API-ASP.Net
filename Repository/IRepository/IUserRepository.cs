using System;
using EcommerceAPI.Models;
using EcommerceAPI.Models.UserDTO;

namespace EcommerceAPI.Repository.IRepository
{
	public interface IUserRepository: IRepository<User>
	{
		public bool isUniqueUser(string Username);

        Task<LoginResponseDTO?> Login(UserLoginDTO loginRequestDTO);

        Task<UserPublicDTO?> Register(UserCreateDTO registerationRequestDTO, string role);

    }
}

