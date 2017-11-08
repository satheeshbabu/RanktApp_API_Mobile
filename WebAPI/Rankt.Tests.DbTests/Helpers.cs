using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Trakker.Api;
using Xunit;
using Program = DatabaseVersioningTool.Program;

namespace Rankt.Tests.DbTests
{
    public static class Helpers
    {
        private static string _connectionString =
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=testmovietvdb;" +
                "Integrated Security=True;Connect Timeout=30;Encrypt=False;" +
                "TrustServerCertificate=True;ApplicationIntent=ReadWrite;" +
                "MultiSubnetFailover=False";

        public static HttpClient StartTestServerAndClient()
        {
            return new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddJsonFile("appsettings.Test.json");
                })
                .UseStartup<Startup>()).CreateClient();
        }


        public static void DropTables()
        {
            Program.DropAllTables(_connectionString);
        }

        public static void AddInitialMigrations()
        {
            Program.AddInitialTestMigrations(_connectionString);
        }

        public static void InitializeMovieDatabase()
        {
            
        }




    }
}