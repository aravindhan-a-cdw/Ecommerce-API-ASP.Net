
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models.DTO.User;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAuthController : ControllerBase
    {
        private readonly ILogger<UserAuthController> _logger;

        public UserAuthController(ILogger<UserAuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Registration")]
        public IActionResult CustomerRegistraion([FromBody] UserCreateDTO UserData)
        {
            return Ok("Hello");
        }

        [HttpPost("Admin/Registration")]
        public IActionResult AdminRegistraion([FromBody] UserCreateDTO UserData)
        {
            return Ok("Hello");
        }

        [HttpPost("Login")]
        public IActionResult UserLogin([FromBody] UserLoginDTO UserData)
        {
            return Ok("Hello");
        }

        [HttpPost("Logout")]
        public IActionResult UserLogout()
        {
            return Ok("Hello");
        }
    }
}

