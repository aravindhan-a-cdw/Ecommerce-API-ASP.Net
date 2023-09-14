using EcommerceAPI.Models;
using EcommerceAPI.Models.UserDTO;

namespace EcommerceAPI.Services.IServices
{
    public interface IUserService
	{
        Task<LoginResponseDTO> Login(UserLoginDTO userData);
        Task<User> Register(UserCreateDTO registerationRequestDTO, string role);
        void Logout(string Token);
    }
}

