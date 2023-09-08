
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models.UserDTO;
using AutoMapper;
using EcommerceAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using EcommerceAPI.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;
using EcommerceAPI.Services.IServices;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{
    /*
     * @author Aravindhan A
     * @description This is the controller class for User related Routes. A new customer or admin account can be generated and that account
     * credentials can be used to obtain JWT token for further requests
     */

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// This is the Controller for User routes to Create, Read or Update both Customer and Admin along with login and logout routes
        /// </summary>
        /// 
        
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public UserController(IMapper mapper, IUserRepository userRepository, IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = contextAccessor;
            _userService = userService;
        }

        // TODO: Check if you need to send cart and orders along
        [Authorize]
        [SwaggerOperation(summary: "Get user details", description: "This endpoint retrieves user details and users orders and cart items")]
        [HttpGet]
        async public Task<IActionResult> GetUser()
        {
            var User = HttpContext.User;
            var user = await _userRepository.GetAsync(record => record.Email == User.Identity.Name, Include: new() {"Orders", "CartItems"});
            return Ok(_mapper.Map<UserPublicDTO>(user));
        }


        [SwaggerOperation(summary: "Create a new Customer", description: "This endpoint allows to register a new Customer")]
        [HttpPost]
        async public Task<IActionResult> CustomerRegistraion([FromBody] UserCreateDTO userData)
        {
            try
            {
                var userDb = await createUser(userData, "customer");
                if (userDb == null)
                {
                    return BadRequest("Data not valid");
                }
                return Ok(_mapper.Map<UserPublicDTO>(userDb));
            } catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }


        /// <summary>
        ///     Create new user and stores in the db
        /// </summary>
        /// <param name="userData">User Details</param>
        /// <param name="role">Role of the User</param>
        /// <returns>User Object</returns>
        /// <exception cref="Exception">Throws exception when data is not acceptable</exception>
        [NonAction]
        async public Task<User?> createUser(UserCreateDTO userData, string role)
        {
            if (!_userRepository.isUniqueUser(userData.Email))
            {
                throw new Exception("Email already exists");
            }
            DateTime today = DateTime.Now;
            DateOnly minDate = new DateOnly(today.Year - 18, today.Month, today.Day);

            if (userData.DateOfBirth > minDate)
            {
                throw new Exception("You are not 18 years old yet! Come back later!");
            }
            return await _userService.Register(userData, role);
        }


        [SwaggerOperation(summary: "Create new Admin", description: "This endpoint allows to create a new Admin user")]
        [HttpPost("admin")]
        async public Task<IActionResult> AdminRegistraion([FromBody] UserCreateDTO userData)
        {
            
                var userDb = await _userService.Register(userData, "admin");
                return Ok(_mapper.Map<UserPublicDTO>(userDb));
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Login user and get access token", description: "This endpoint allows users to login in and get access token")]
        [HttpPost("Login")]
        async public Task<IActionResult> UserLogin([FromBody] UserLoginDTO userData)
        {
            /// <summary>
            /// Route to login user
            /// </summary>
            
            var user = await _userRepository.Login(userData);
            return Ok(user);
        }


        [SwaggerOperation(summary: "Logout user and invalidate the token", description: "This endpoint allows user to logout and invalidate the access token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Logout")]
        public IActionResult UserLogout()
        {
            /// <summary>
            /// Route to logout user
            /// </summary>

            var JWTToken = _httpContextAccessor.HttpContext.Request.Headers.Authorization.FirstOrDefault().Split(" ")[1];
            _userService.Logout(JWTToken);
            return NoContent();
        }
    }
}

