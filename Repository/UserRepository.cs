using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Models.UserDTO;
using EcommerceAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceAPI.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
	{

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly string SecretKey;

        public UserRepository(ApplicationDbContext dbContext, IMapper mapper, IConfiguration configuration, RoleManager<IdentityRole> roleManager,UserManager<User> userManager): base(dbContext)
		{
            _db = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            if(configuration.GetValue<string>("ApiSettings:Secret") == null) {
                throw new Exception("Secret Key has not been set!");
            }
            SecretKey = configuration.GetValue<string>("ApiSettings:Secret") ?? "";
        }

        public bool isUniqueUser(string Email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(record => record.Email == Email.ToLower());
            return user == null;
        }

        public async Task<LoginResponseDTO?> Login(UserLoginDTO userData)
		{
            var user = _db.ApplicationUsers.FirstOrDefault(record => record.Email == userData.Email);
            bool isValid = await _userManager.CheckPasswordAsync(user, userData.Password);
            
            if (user == null || isValid == false)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new()
            {
                token = tokenHandler.WriteToken(token),
                user = _mapper.Map<UserPublicDTO>(user),

            };
            return loginResponseDTO;
        }

        async public Task<User?> Register(UserCreateDTO registerationRequestDTO, string role) 
        {
            User user = _mapper.Map<User>(registerationRequestDTO);
            user.UserName = registerationRequestDTO.Email;

            var result = _userManager.CreateAsync(user, registerationRequestDTO.Password);

            if (result.Result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
                var userToReturn = _db.ApplicationUsers
                    .FirstOrDefault(u => u.UserName == registerationRequestDTO.Email);
                return userToReturn;
            }

            return null;
        }
    }
}

