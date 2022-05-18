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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AddressController : Controller
    {
        private readonly ApiDbContext _context;
        private readonly JwtTokenUtil _jwt;
        private readonly ILogger<DefaultController> _logger;

        public AddressController(ILogger<DefaultController> logger, ApiDbContext context, JwtTokenUtil jwt)
        {
            _context = context;
            _logger = logger;
            _jwt = jwt;
        }

        [Authorize]
        [HttpPost("/address")]
        public async Task<Address> CreateAddress([FromBody] Address address)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var token = accessToken;
            var username = _jwt.GetUsernameFromToken(token);
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);

            var _address = new Address
            {
                road = address.road, postalCode = address.postalCode, city = address.city, country = address.country
            };
            _address.creationDate = DateTime.Now;
            _address.updatedDate = DateTime.Now;
            _address.User = currentUser.ID;

            var test = _context.Set<Address>();
            test.Add(_address);
            _context.SaveChanges();
            return _address;
            //return StatusCode(201 , "Street "  +  _address.road  + " City " + _address.City + " was Created!");
        }

        [Authorize]
        [HttpGet("/address")]
        public async Task<IEnumerable<Address>> GetAddress()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var token = accessToken;
            var username = _jwt.GetUsernameFromToken(token);
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);


            var list = _context.Address.ToList();
            var goodlist = list.Where(r => r.User == currentUser.ID);
            return goodlist;
            //return StatusCode(201 , "Street "  +  _address.road  + " City " + _address.City + " was Created!");
        }

        [Authorize]
        [HttpGet("/address/{id}")]
        public async Task<Address> GetAddressById(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var token = accessToken;
            var username = _jwt.GetUsernameFromToken(token);
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);

            var currentAddress = _context.Address.FirstOrDefault(u => u.id == id);
            if (currentAddress == null)
                return null;
            return currentAddress;
            //return StatusCode(201 , "Street "  +  _address.road  + " City " + _address.City + " was Created!");
        }

        [Authorize]
        [HttpPut("/address/{id}")]
        public async Task<Address> ChangeAddress(int id, [FromBody] Address address)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var token = accessToken;
            var username = _jwt.GetUsernameFromToken(token);
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);

            var currentAddress = _context.Address.FirstOrDefault(u => u.id == id);
            if (currentAddress == null)
                return null;

            if (address.road != null)
                currentAddress.road = address.road;
            if (address.city != null)
                currentAddress.city = address.city;
            if (address.postalCode != null)
                currentAddress.postalCode = address.postalCode;
            if (address.country != null)
                currentAddress.country = address.country;

            if (currentUser.Role.Equals(UserRole.ROLE_ADMIN) || currentUser.ID == currentAddress.User)
            {
                _context.Update(currentAddress);
                await _context.SaveChangesAsync();
                return currentAddress;
            }

            return null;
        }

        [Authorize]
        [HttpDelete("/address/{id}")]
        public async Task<string> DeleteAddressById(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var token = accessToken;
            var username = _jwt.GetUsernameFromToken(token);
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == username);

            var currentAddress = _context.Address.FirstOrDefault(u => u.id == id);
            if (currentAddress == null)
                return "Echec";
            if (currentUser.Role.Equals(UserRole.ROLE_ADMIN) || currentUser.ID == currentAddress.User)
            {
                _context.Remove(currentAddress);
                await _context.SaveChangesAsync();
                return "Success";
            }

            return "Error";
        }
    }
}