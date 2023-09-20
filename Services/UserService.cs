using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using EcommerceAPI.Controllers;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Models.UserDTO;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace EcommerceAPI.Services
{
    /*
     * @author Aravindhan A
     * @description This is a UserService for Login and User Registration
     */
    public class UserService: IUserService
	{
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly ApplicationDbContext _db;
        internal DbSet<User> _dbSet;
        private readonly IMapper _mapper;
        private readonly string SecretKey;
        private ILogger<UserController> _logger;
        private readonly IConnectionMultiplexer _redis;


        public UserService(ApplicationDbContext dbContext, IMapper mapper, IConfiguration configuration,
            RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ILogger<UserController> logger, IConnectionMultiplexer redis)
		{
            //_db = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _redis = redis;
            _logger = logger;

            _dbSet = dbContext.Set<User>();

            if (configuration.GetValue<string>("ApiSettings:Secret") == null)
            {
                throw new Exception("Secret Key has not been set!");
            }
            SecretKey = configuration.GetValue<string>("ApiSettings:Secret") ?? "";
        }

        async public Task<LoginResponseDTO> Login(UserLoginDTO userData)
        {
            var query = _dbSet.Include("Orders").Include("CartItems");
            var user = query.FirstOrDefault(record => record.Email == userData.Email);

            bool isValid = user == null || await _userManager.CheckPasswordAsync(user, userData.Password);

            if (isValid == false || user == null)
            {
                throw new BadHttpRequestException("Username or password doesn't match", StatusCodes.Status404NotFound);
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

        async public Task<User> Register(UserCreateDTO registerationRequestDTO, string role)
        {
            User user = _mapper.Map<User>(registerationRequestDTO);
            user.UserName = registerationRequestDTO.Email;

            // Did you notice that I have called a async function without await
            var result = _userManager.CreateAsync(user, registerationRequestDTO.Password);

            if (result.Result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
                
                var userToReturn = _dbSet.Include("Orders").Include("CartItems").FirstOrDefault(u => u.UserName == registerationRequestDTO.Email);
                if(userToReturn == null)
                {
                    throw new BadHttpRequestException("Some error occurred while registering user");
                }
                return userToReturn;
            }
            else if(result.Exception != null)
            {
                throw result.Exception;
            }
            _logger.LogError("You have got an unhandled path or unknown code flow", result);
            throw new Exception("Some Unidentified error");
        }

        async public void Logout(string Token)
        {
            // Get redis database and insert the token with expiry time of 30 mins
            var db = _redis.GetDatabase();
            await db.StringSetAndGetAsync(Token, new RedisValue(""), new TimeSpan(0, minutes: 30, 0));
        }
    }
}

