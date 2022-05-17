using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using quest_web;
using quest_web.Controllers;
using quest_web.Models;
using quest_web.Utils;
using Xunit;

namespace TestProject1
{
    public class TestAuthentification
    {
        
        private readonly JwtTokenUtil _jwt;
        private DbContextOptionsBuilder<ApiDbContext> dbContextOptions = new DbContextOptionsBuilder<ApiDbContext>();

        public TestAuthentification()
        {
            _jwt = new JwtTokenUtil();
        }
        
        [Fact]
        public async Task Test_Register_POST()
        {
            // Arrange
            var user = new User()
            {
                Username = "Drissa",
                Password = "123",
            };
            // user.Creation_Date = DateTime.Now;
            // user.Updated_Date = user.Creation_Date;

            var mockRepo = new Mock<ApiDbContext>();
            mockRepo.Setup(repo => repo.Set<User>()).Returns(user);
            
            var controller = new AuthenticationController(mockRepo.Object, _jwt);

            // Act
            var result = controller.Register(user);
            
            // Assert
            Console.WriteLine(result);
        }
    }
}