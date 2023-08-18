using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerAuthController : ControllerBase
    {
        private readonly ILogger<CustomerAuthController> _logger;

        public CustomerAuthController(ILogger<CustomerAuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Registration")]
        public IActionResult CustomerRegistraion()
        {
            return Ok("Hello");
        }

        [HttpPost("Login")]
        public IActionResult CustomerLogin()
        {
            return Ok("Hello");
        }

        [HttpPost("Logout")]
        public IActionResult CustomerLogout()
        {
            return Ok("Hello");
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class AdminAuthController : ControllerBase
    {
        private readonly ILogger<AdminAuthController> _logger;

        public AdminAuthController(ILogger<AdminAuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Registration")]
        public IActionResult CustomerRegistraion()
        {
            return Ok("Hello");
        }

        [HttpPost("Login")]
        public IActionResult CustomerLogin()
        {
            return Ok("Hello");
        }

        [HttpPost("Logout")]
        public IActionResult CustomerLogout()
        {
            return Ok("Hello");
        }
    }
}

