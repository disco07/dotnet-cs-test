using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quest_web;
using quest_web.Utils;
using Xunit;
using Xunit.Abstractions;

namespace TestProject1
{
    public class TestDefaultController
    {
        private readonly JwtTokenUtil _jwt;
        private readonly ITestOutputHelper _output;
        private readonly ApiDbContext _context;
        private readonly string _myuuidAsString;
        private readonly HttpClient _client;
        
        public TestDefaultController(ITestOutputHelper output)
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

            _client = new TestClientProvider().Client;
        }
        
        [Fact]
        public async Task Test_testSuccess()
        {
            // Arrange
            var response = await _client.GetAsync("/testSuccess");

            // Act
            response.EnsureSuccessStatusCode();
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task Test_testNotFound()
        {
            // Arrange
            var response = await _client.GetAsync("/testNotFound");
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        
        [Fact]
        public async Task Test_testError()
        {
            // Arrange
            var response = await _client.GetAsync("/testError");
            
            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}