using System;
using System.Net.Http;
using quest_web;
using quest_web.Utils;
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
        
        
    }
}