using System;
using System.Collections.Generic;
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
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ApiDbContext _context;
        private readonly JwtTokenUtil _jwt;

        public UserController(ApiDbContext context, JwtTokenUtil jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        [Authorize]
        [HttpGet("/user")]
        public async Task<IEnumerable<User>> GetUser()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            string token = accessToken.ToString();
            var currentUser = _context.Users.FirstOrDefault(u => u.Username ==  _jwt.GetUsernameFromToken(token));

            // if (currentUser == null)
            // {
            //     return NotFound();
            // }

            var list = _context.Users.ToList();
            if (list == null)
                return null;
            return list;
        }

        [Authorize]
        [HttpGet("/user/{id}")]
        public async Task<User> GetUserById(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            string token = accessToken.ToString();
            var username = _jwt.GetUsernameFromToken(token);
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);

            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (user == null)
                return null;
            return user;
            //return StatusCode(201 , "Street "  +  _address.road  + " City " + _address.City + " was Created!");
        }

        [Authorize]
        [HttpPut("/user/{id}")]
        public async Task<User> ChangeAddress(int id, [FromBody] User address)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            string token = accessToken.ToString();
            var username = _jwt.GetUsernameFromToken(token);
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);

            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (user == null)
                return null;
            
            user.Role = address.Role;
            if (address.Username != null)
                user.Username = address.Username;

            if (currentUser.Role.Equals(UserRole.ROLE_ADMIN) || currentUser.ID == user.ID) {
                _context.Update(user);
                await _context.SaveChangesAsync();

                return user;
            }
            return null;
        }

        [Authorize]
        [HttpDelete("/user/{id}")]
        public async Task<String> DeleteAddressById(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            string token = accessToken.ToString();
            var username = _jwt.GetUsernameFromToken(token);
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);

            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (user == null)
                return "Echec";
            if (currentUser.Role.Equals(UserRole.ROLE_ADMIN) || currentUser.ID == user.ID ) {
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return "Success";
            }
            return "Error";
        }
        
    }
}