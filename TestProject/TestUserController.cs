using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using quest_web;
using quest_web.Models;
using quest_web.Utils;
using Xunit;
using Xunit.Abstractions;

namespace TestProject1
{
    public class TestUserController
    {
        private readonly HttpClient _client;
        private readonly ApiDbContext _context;
        private readonly JwtTokenUtil _jwt;
        private readonly string _myuuidAsString;
        private readonly ITestOutputHelper _output;

        public TestUserController(ITestOutputHelper output)
        {
            _jwt = new JwtTokenUtil();

            _output = output;

            var myuuid = Guid.NewGuid();
            _myuuidAsString = myuuid.ToString();

            _client = new TestClientProvider().Client;
        }
        
        [Fact]
        public async Task Test_GetUser_Bad_Request()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/user");

            _output.WriteLine(response.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async Task Test_GetUser_Work()
        {
            // Arrange
            var user = new User
            {
                Username = "drissakone",
                Password = "123"
            };
            
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            await _client.PostAsync(
                "/register",
                content
            );
            var login = await _client.PostAsync(
                "/authenticate",
                content
            );

            // Act
            var responseContent = await login.Content.ReadAsStringAsync();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseContent.Replace("\"", ""));

            var response = await _client.GetAsync("/user");

            _output.WriteLine(response.ToString());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}