using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rankt.Api.Repositories.Lists;
using Rankt.Api.Repositories.Users;
using Trakker.Api.Repositories.Movies;
using Trakker.Api.Singletons;
using Trakker.Api.StartUp;
using TrakkerApp.Api.Repositories.Relations;

namespace Rankt.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // ********************
            // Setup CORS
            // ********************
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin(); // For anyone access.
            //corsBuilder.WithOrigins("http://localhost:56573"); // for a specific url. Don't add a forward slash on the end!
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMemoryCache();

            services.AddSingleton(_ => Configuration);
            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<IMediaListRepository, MediaListRepository>();
            services.AddTransient<IRelationRepository, RelationRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMemoryCache cache)
        {
            //Need to initialize the list of cache
            TrakkerCache.InitializeCacheList();

//            Console.WriteLine(Configuration.GetConnectionString("DefaultConnection"));
//            string cs = Configuration["ConnectionStrings:DefaultConnection"];

            var loadDataTask = StartUpTasks.TasksOnStartUp(Configuration, cache);
            loadDataTask.Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-GB"),
                new CultureInfo("en-US"),
                new CultureInfo("en"),
                new CultureInfo("fr-FR"),
                new CultureInfo("fr"),
                new CultureInfo("ml-IN"),
                new CultureInfo("hi-IN")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };

            app.UseRequestLocalization(options);
            
            loggerFactory.AddDebug();

            app.UseCors("SiteCorsPolicy");

            app.UseAuthentication();
            app.UseMvc();

        }
    }
}
