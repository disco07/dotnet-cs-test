using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

            const string connectionString = "server=localhost;port=3306;database=quest_web;user=drissa;password=root;";
            var builder = new DbContextOptionsBuilder<ApiDbContext>();
            builder.UseMySql(connectionString);
            var options = builder.Options;
            _context = new ApiDbContext(options);

            var myuuid = Guid.NewGuid();
            _myuuidAsString = myuuid.ToString();

            _client = new TestClientProvider().Client;
            _client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Fact]
        public async Task Test_Register_POST_Bad_Request()
        {
            // Arrange
            var user = new User
            {
                Username = "Kone"
            };
            
            // Act
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(
                "/register", 
                content
                );

            _output.WriteLine(response.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            
            // Act
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            await _client.PostAsync(
                "/register", 
                content
            );
            
            var response = await _client.PostAsync(
                "/register", 
                content
            );

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
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
            
            // Act
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(
                "/register", 
                content
            );
            
            _output.WriteLine(response.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
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

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            await _client.PostAsync(
                "/register",
                content
            );
            var response = await _client.PostAsync(
                "/authenticate",
                content
            );
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Authenticate_not_Work_Invalid_Password()
        {
            // Arrange
            var user = new User
            {
                Username = "Kone",
                Password = "1234fdsff"
            };

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(
                "/authenticate",
                content
            );
            _output.WriteLine(response.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(
                "/authenticate",
                content
            );
            _output.WriteLine(response.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Test_Me_Access_Denied()
        {
            // Arrange
            var response = await _client.GetAsync("/me");
            
            _output.WriteLine(response.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}