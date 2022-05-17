using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using quest_web.Models;
using quest_web.Utils;

namespace quest_web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly ILogger<DefaultController> _logger;
        private readonly ApiDbContext _context;
        private readonly JwtTokenUtil _jwt;

        public AuthenticationController(ILogger<DefaultController> logger, ApiDbContext context, JwtTokenUtil jwt)
        {
            _context = context;
            _logger = logger;
            _jwt = jwt;
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public IActionResult Register([FromBody]User user)
        {
            User _user = new User {Username = user.Username, Password = user.Password, Role = user.Role};
            _user.Creation_Date = DateTime.Now;
            _user.Updated_Date = _user.Creation_Date;
             
            if (String.IsNullOrEmpty(_user.Role.ToString()) == true)
                _user.Role = 0;
            
            var use = _context.Users.Where(u => u.Username == user.Username).FirstOrDefault();
            if (String.IsNullOrEmpty(_user.Username) == true || String.IsNullOrEmpty(_user.Password) == true) {
                return BadRequest("Username or password expected value but is none!");
            } else if (use!=null) {
                return Conflict("Error username already exist!!!");
            } else {
                _user.Password = BCrypt.Net.BCrypt.HashPassword(_user.Password);
                var test = _context.Set<User>();
                test.Add(_user);
                _context.SaveChanges();
                 return StatusCode(201 , "User "  +  _user.Username  + " Role " + _user.Role.ToString() + " was Created!");
            }
        }

        [AllowAnonymous]
        [HttpPost("/authenticate")]
        public IActionResult Authenticate([FromBody]User user)
        {
            UserDetails us = new UserDetails {Username = user.Username, Password = user.Password, Role = user.Role};
            try {
                var currentUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
                if (BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password) == false)
                    return StatusCode(401 , "Password is not valid");
                return Ok(_jwt.GenerateToken(us));
            } catch (Exception ex) {
                Console.WriteLine(ex);
                return StatusCode(401 , "User doesn't exist");
            }
        }

        [Authorize]
        [HttpGet("/me")]
        public async Task<UserDetails> Me()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var currentUser = _context.Users.FirstOrDefault(u => u.Username == _jwt.GetUsernameFromToken(accessToken.ToString()));
            UserDetails us =  new UserDetails {Username = currentUser.Username, Role = currentUser.Role};

            return us;
        }

    }
}