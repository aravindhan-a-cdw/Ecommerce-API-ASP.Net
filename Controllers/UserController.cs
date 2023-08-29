
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models.UserDTO;
using AutoMapper;
using EcommerceAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // TODO: Check if you need to send cart and orders along
        [HttpGet]
        [Authorize]
        async public Task<IActionResult> GetUser()
        {
            var User = HttpContext.User;
            var user = await _userRepository.GetAsync(record => record.Email == User.Identity.Name);
            return Ok(_mapper.Map<UserPublicDTO>(user));
        }

        [HttpPost]
        async public Task<IActionResult> CustomerRegistraion([FromBody] UserCreateDTO userData)
        {
            Console.WriteLine(userData);
            if(!_userRepository.isUniqueUser(userData.Email))
            {
                return BadRequest("Email already exists");
            }
            DateTime today = DateTime.Now;
            DateOnly minDate = new DateOnly(today.Year - 18, today.Month, today.Day);

            if( userData.DateOfBirth > minDate)
            {
                return BadRequest("You are not 18 years old yet! Come back later!");
            }
            var userDb = await _userRepository.Register(userData, "customer");

            if (userDb == null)
            {
                return BadRequest("Data not valid");
                //throw new Exception("User Could not be created");
            }

            return Ok(_mapper.Map<UserPublicDTO>(userDb));
        }

        [HttpPost("admin")]
        async public Task<IActionResult> AdminRegistraion([FromBody] UserCreateDTO userData)
        {
            Console.WriteLine(userData);
            var userDb = await _userRepository.Register(userData, "admin");

            if (userDb == null)
            {
                throw new Exception("User Could not be created");
            }

            return Ok(_mapper.Map<UserPublicDTO>(userDb));
        }

        [HttpPost("Login")]
        async public Task<IActionResult> UserLogin([FromBody] UserLoginDTO userData)
        {
            var user = await _userRepository.Login(userData);
            if (user == null)
            {
                return BadRequest("Username and Password doesn't match!");
            }
            return Ok(user);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Logout")]
        public IActionResult UserLogout()
        {
            //HttpContext.Session.Clear();
            return Ok("Hello");
        }
    }
}

