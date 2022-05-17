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
        private ApiDbContext dbContextOptions;
        
        public TestAuthentification()
        {
            _jwt = new JwtTokenUtil();
        }
        
        [Fact]
        public async Task Test_Register_POST()
        {
            const string connectionString = "server=127.0.0.1;database=quest_web;user=root;password=;";
            var builder = new DbContextOptionsBuilder<ApiDbContext>();
            builder.UseMySql(connectionString);
            var options = builder.Options;
            var context = new ApiDbContext(options);
            
            // Arrange
            var user = new User()
            {
                Username = "Drissa",
                Password = "123",
            };
            // user.Creation_Date = DateTime.Now;
            // user.Updated_Date = user.Creation_Date;

            var mockRepo = new Mock<ApiDbContext>();
            mockRepo.Setup(repo => repo.Set<User>());
            
            var controller = new AuthenticationController(context, _jwt);

            // Act
            var result = controller.Register(user);
            
            // Assert
            Console.WriteLine(result);
        }
    }
}