using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Trakker.Api;
using Xunit;
using Program = DatabaseVersioningTool.Program;

namespace Rankt.IntegrationTests.TestDatabase
{
    public class InitialTests
    {
        private TestServer _server;
        private HttpClient _client;

        private string connectionString =
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=testmovietvdb;" +
                "Integrated Security=True;Connect Timeout=30;Encrypt=False;" +
                "TrustServerCertificate=True;ApplicationIntent=ReadWrite;" +
                "MultiSubnetFailover=False"
            ;

        public void StartUp()
        {
            //Drop all Tables in DB
            Program.DropAllTables(connectionString);
            //Add initial Migrations
            Program.AddInitialTestMigrations(connectionString);
            //Start Server
            _server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddJsonFile("appsettings.Test.json");
                })
                .UseStartup<Startup>());

            _client = _server.CreateClient();

        }

        [Fact]
        public async Task Test1()
        {
            StartUp();
            var response = await _client.GetAsync("/api/movie");
            response.EnsureSuccessStatusCode();
            var respString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(respString);
        }
    }
}