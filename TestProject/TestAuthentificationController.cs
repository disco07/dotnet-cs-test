using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Moq;
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
        
        private readonly JwtTokenUtil _jwt;
        private readonly ITestOutputHelper _output;
        private readonly ApiDbContext _context;
        private readonly string _myuuidAsString;

        public TestAuthentification(ITestOutputHelper output)
        {
            _jwt = new JwtTokenUtil();
            _output = output;
            const string connectionString = "server=127.0.0.1;database=quest_web;user=root;password=;";
            var builder = new DbContextOptionsBuilder<ApiDbContext>();
            builder.UseMySql(connectionString);
            var options = builder.Options;
            _context = new ApiDbContext(options);
            Guid myuuid = Guid.NewGuid();
            _myuuidAsString = myuuid.ToString();
        }
        
        [Fact]
        public async Task Test_Register_POST_Bad_Request()
        {
            // Arrange
            var user = new User()
            {
                Username = "Kone",
            };
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            var result = controller.Register(user);
            
            // Assert
            BadRequestObjectResult objectResponse = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, objectResponse.StatusCode);
        }
        
        [Fact]
        public async Task Test_Register_POST_Not_Work_Conflict()
        {
            // Arrange
            var user = new User()
            {
                Username = "Kone" + _myuuidAsString,
                Password = "123",
            };
            _output.WriteLine(_myuuidAsString);
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            controller.Register(user);
            var result = controller.Register(user);
            
            // Assert
            ConflictObjectResult objectResponse = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, objectResponse.StatusCode);
        }
        
        [Fact]
        public async Task Test_Register_POST_Work()
        {
            // Arrange
            var user = new User()
            {
                Username = "Kone" + _myuuidAsString,
                Password = "123",
            };
            _output.WriteLine(_myuuidAsString);
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            var result = controller.Register(user);
            
            // Assert
            ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResponse.StatusCode);
        }
        
        [Fact]
        public async Task Test_Authenticate_Work()
        {
            // Arrange
            var user = new User()
            {
                Username = "drissakone",
                Password = "123",
            };
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            controller.Register(user);
            var result = controller.Authenticate(user);
            
            // Assert
            OkObjectResult objectResponse = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResponse.StatusCode);
            _output.WriteLine(result.ToString());
        }
        
        [Fact]
        public async Task Test_Authenticate_not_Work_Invalid_Password()
        {
            // Arrange
            var user = new User()
            {
                Username = "Kone",
                Password = "1234",
            };
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            var result = controller.Authenticate(user);
            
            // Assert
            ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(401, objectResponse.StatusCode);
        }
        
        [Fact]
        public async Task Test_Authenticate_not_Work_Invalid_USER()
        {
            // Arrange
            var user = new User()
            {
                Username = "invaliduser",
                Password = "1234",
            };
            var controller = new AuthenticationController(_context, _jwt);

            // Act
            var result = controller.Authenticate(user);
            
            // Assert
            ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(401, objectResponse.StatusCode);
        }
        
        // [Fact]
        // public async Task Test_Me_Work()
        // {
        //     // Arrange
        //     var controller = new AuthenticationController(_context, _jwt);
        //
        //     // Act
        //     var result = controller.Me();
        //     _output.WriteLine(result.Result.ToString());
        //     
        //     // Assert
        //     // ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
        //     // Assert.Equal(401, objectResponse.StatusCode);
        // }
    }
}