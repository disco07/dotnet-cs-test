using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using quest_web;
using quest_web.Controllers;
using quest_web.Models;
using quest_web.Utils;
using Xunit;
using Xunit.Abstractions;

namespace TestProject1
{
    public class TestAuthentification
    {
        private readonly HttpClient _client;
        private readonly ApiDbContext _context;

        private readonly JwtTokenUtil _jwt;
        private readonly string _myuuidAsString;
        private readonly ITestOutputHelper _output;

        public TestAuthentification(ITestOutputHelper output)
        {
            _jwt = new JwtTokenUtil();

            _output = output;

            const string connectionString = "server=127.0.0.1;database=quest_web;user=root;password=;";
            var builder = new DbContextOptionsBuilder<ApiDbContext>();
            builder.UseMySql(connectionString);
            var options = builder.Options;
            _context = new ApiDbContext(options);

            var myuuid = Guid.NewGuid();
            _myuuidAsString = myuuid.ToString();

            _client = new TestClientProvider().Client;
        }

        [Fact]
        public async Task Test_Register_POST_Bad_Request()
        {
            // Arrange
            var user = new User
            {
                Username = "Kone"
            };
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            var result = controller.Register(user);

            // Assert
            var objectResponse = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Register_POST_Not_Work_Conflict()
        {
            // Arrange
            var user = new User
            {
                Username = "Kone" + _myuuidAsString,
                Password = "123"
            };
            _output.WriteLine(_myuuidAsString);
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            controller.Register(user);
            var result = controller.Register(user);

            // Assert
            var objectResponse = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Register_POST_Work()
        {
            // Arrange
            var user = new User
            {
                Username = "Kone" + _myuuidAsString,
                Password = "123"
            };
            _output.WriteLine(_myuuidAsString);
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            var result = controller.Register(user);

            // Assert
            var objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Authenticate_Work()
        {
            // Arrange
            var user = new User
            {
                Username = "drissakone",
                Password = "123"
            };
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            controller.Register(user);
            var result = controller.Authenticate(user);

            // Assert
            var objectResponse = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResponse.StatusCode);
            _output.WriteLine(result.ToString());
        }

        [Fact]
        public async Task Test_Authenticate_not_Work_Invalid_Password()
        {
            // Arrange
            var user = new User
            {
                Username = "Kone",
                Password = "1234"
            };
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            var result = controller.Authenticate(user);

            // Assert
            var objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(401, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Authenticate_not_Work_Invalid_USER()
        {
            // Arrange
            var user = new User
            {
                Username = "invaliduser",
                Password = "1234"
            };
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            var result = controller.Authenticate(user);

            // Assert
            var objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(401, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Me_Access_Denied()
        {
            // Arrange
            var response = await _client.GetAsync("/me");
            
            _output.WriteLine(response.ToString());
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}