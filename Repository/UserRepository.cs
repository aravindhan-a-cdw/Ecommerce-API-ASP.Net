using System;
using EcommerceAPI.Models;
using EcommerceAPI.Data;
using EcommerceAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models.UserDTO;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

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

        public override async Task<User?> GetAsync(System.Linq.Expressions.Expression<Func<User, bool>> Filter, bool NoTracking = false)
        {
            IQueryable<User> query = dbSet;

            if (NoTracking)
            {
                query = query.AsNoTracking();
            }
            query = query.Include(record => record.CartItems).Include(record => record.Orders);
            if (Filter != null)
            {
                query = query.Where(Filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<LoginResponseDTO?> Login(UserLoginDTO userData)
		{
            var user = _db.ApplicationUsers.FirstOrDefault(record => record.Email == userData.Email);
            bool isValid = await _userManager.CheckPasswordAsync(user, userData.Password);
            //var tokenNew = await _userManager.GenerateUserTokenAsync(user, "Demo", "Just checking");

            //var loginInfo = new UserLoginInfo("Demo", SecretKey, "Demo");
            //await _userManager.AddLoginAsync(user, loginInfo);
            //var tokenNew = await _userManager.CreateSecurityTokenAsync(user);
            
            
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

        async public Task<UserPublicDTO?> Register(UserCreateDTO registerationRequestDTO, string role)
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
                return _mapper.Map<UserPublicDTO>(userToReturn);
            }

            return null;
        }

        async public Task Logout()
        {

        }
    }
}

