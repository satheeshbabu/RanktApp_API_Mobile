using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DatabaseVersioningTool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Rankt.Api;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
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
            InitializeMovieGenres();
            var migrations = GetNewMigrations("DBInitializations\\Movie");
            Program.RunMigrations(_connectionString, migrations);
        }

        public static void InitializeMovieGenres()
        {
            var migrations = GetNewMigrations("DBInitializations\\MovieGenres");
            Program.RunMigrations(_connectionString, migrations);
        }






        private static  IReadOnlyList<Migration> GetNewMigrations(string path)
        {

            return new DirectoryInfo(path + "\\")
                .GetFiles()
                .Select(x => new Migration(x))
                .ToList();
        }

    }
}