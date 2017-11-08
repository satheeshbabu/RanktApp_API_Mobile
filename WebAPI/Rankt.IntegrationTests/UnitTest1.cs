using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Trakker.Api;
using Xunit;
using Program = DatabaseVersioningTool.Program;

namespace Rankt.IntegrationTests
{
    public class UnitTest1
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public UnitTest1()
        {
            _server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddJsonFile("appsettings.json");
                    configBuilder.Properties.TryGetValue("ConnectionStrings:DefaultConnection", out Object conString);
                    string con = conString.ToString();
                })
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Test1()
        {
            Program.AddMigrations();
            var response = await _client.GetAsync("/api/movie");
            response.EnsureSuccessStatusCode();
            var respString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(respString);
        }
    }
}
