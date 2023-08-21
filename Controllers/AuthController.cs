
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models.DTO.User;
using AutoMapper;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Repository;
using EcommerceAPI.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAuthController : ControllerBase
    {
        private readonly ILogger<UserAuthController> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public UserAuthController(ILogger<UserAuthController> logger, IMapper mapper, IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        [HttpPost("Registration")]
        async public Task<IActionResult> CustomerRegistraion([FromBody] UserCreateDTO UserData)
        {
            Console.WriteLine(UserData);
            var UserDb = await _userRepository.CreateAsync(_mapper.Map<User>(UserData));
            var CustomerRole = await _roleRepository.GetAsync(record => record.Name == "Customer");
            if(CustomerRole == null)
            {
                throw new Exception("Customer Role is not added in the Database. Kindly add Customer Role before creating customer!");
            }
            UserDb.Roles.Add(CustomerRole);
            UserDb = await _userRepository.UpdateAsync(UserDb);

            return Ok(_mapper.Map<UserPublicDTO>(UserDb));
        }

        [HttpPost("Admin/Registration")]
        async public Task<IActionResult> AdminRegistraion([FromBody] UserCreateDTO UserData)
        {
            Console.WriteLine(UserData);
            var UserDb = await _userRepository.CreateAsync(_mapper.Map<User>(UserData));
            var AdminRole = await _roleRepository.GetAsync(record => record.Name == "Admin");

            if (AdminRole == null)
            {
                throw new Exception("Admin Role is not added in the Database. Kindly add Admin Role before creating a Admin User!");
            }
            UserDb.Roles.Add(AdminRole);
            UserDb = await _userRepository.UpdateAsync(UserDb);

            return Ok(_mapper.Map<UserPublicDTO>(UserDb));
        }

        [HttpPost("Login")]
        async public Task<IActionResult> UserLogin([FromBody] UserLoginDTO UserData)
        {
            var user = await _userRepository.GetAsync(record => record.Email == UserData.Email && record.Password == UserData.Password);
            if (user == null)
            {
                return BadRequest("Username and Password doesn't match!");
            }
            return Ok("Have Your Token");
        }

        [HttpPost("Logout")]
        public IActionResult UserLogout()
        {
            return Ok("Hello");
        }
    }
}

