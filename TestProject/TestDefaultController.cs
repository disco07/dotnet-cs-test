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
        private readonly HttpClient _client;
        
        public TestDefaultController()
        {
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