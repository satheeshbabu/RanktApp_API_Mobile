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
        //TODO this also uses the cache.... maybe need to determine a way to test that
        //TODO using the response time,  maybe something like this
        //https://stackoverflow.com/questions/14177725/how-to-get-httpclient-response-time-when-running-in-parallel
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
            response.EnsureSuccessStatusCode();
            var respString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(respString);
            var numberOfMovies = (int) jsonResponse["number_movies"];
            Assert.True(numberOfMovies > 0);
        }

        [Fact]
        public async Task Ensure_Success_Response_From_Get_Existing_Movie()
        {
            StartUp();
            var response = await _client.GetAsync("/api/movie/3");
            response.EnsureSuccessStatusCode();
            
            var respString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(respString);
            var genreArray = (JArray)jsonResponse["genres"];
            Assert.True(genreArray.Count > 0);

            var response2 = await _client.GetAsync("/api/movie/3");
            response2.EnsureSuccessStatusCode();
            var respString2 = await response2.Content.ReadAsStringAsync();
            var jsonResponse2 = JObject.Parse(respString2);
            var genreArray2 = (JArray)jsonResponse2["genres"];
            Assert.True(genreArray2.Count > 0);
        }
    }
}