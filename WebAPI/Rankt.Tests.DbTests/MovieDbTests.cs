using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json.Linq;
using Trakker.Api;
using Xunit;
using Program = DatabaseVersioningTool.Program;

namespace Rankt.Tests.DbTests
{
    public class MovieDbTests
    {
        private HttpClient _client;

        public void StartUp()
        {
            //Drop all Tables in DB
            Helpers.DropTables();
            //Add initial Migrations
            Helpers.AddInitialMigrations();
            //Start Server
            _client = Helpers.StartTestServerAndClient();
            Helpers.InitializeMovieDatabase();
        }

        [Fact]
        public async Task Ensure_Success_Response_From_Get_All_Movies()
        {
            StartUp();
            var response = await _client.GetAsync("/api/movie");
            //response.EnsureSuccessStatusCode();
            var respString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(respString);

        }
    }
}