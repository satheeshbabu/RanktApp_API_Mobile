using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Trakker.Api.Repositories.Genres.MovieGenres;
using Trakker.Api.Repositories.Movies;
using TrakkerApp.Api.Repositories.Categories;
using TrakkerApp.Api.Repositories.CategoryRelations;
using TrakkerApp.Api.Repositories.Genres.MovieGenres;
using TrakkerApp.Api.Repositories.Genres.TVShowGenres;
using TrakkerApp.Api.StartUp;

namespace Trakker.Api.StartUp
{
    public static class StartUpTasks
    {
        public const string CANCELLATION_TOKEN_KEY = "CANCELLATION_TOKEN";

        public static async Task TasksOnStartUp(IConfiguration configuration, IMemoryCache memoryCache)
        {
            var categoryMapping = new CategoryMapping(new CategoryRepository(configuration),
                new CategoryRelRepository(configuration));

            await categoryMapping.PopulateAllCategories();

            var movieRepo = new MovieRepository(configuration, memoryCache);

            var movies = await movieRepo.GetAllMovies();

            if (!configuration.GetConnectionString("DefaultConnection").Contains("test"))
            {
                await MovieGenrePopulating.PopulateTmdbMovieGenres(new MovieGenreRepository(configuration,
                    memoryCache));
                await TVShowGenrePopulating.PopulateTmdbTVShowGenres(new TVShowGenreRepository(configuration));

                await new MoviePopulating(configuration, memoryCache).PopulateManyMovies();

                await new TVShowPopulating(configuration, memoryCache).PopulateManyTVShowsTask();
//
//            var entity = await BaseModelResolver.ResolveBaseEntity(Movie.ENTITY_CATEGORY_ID, 32);
//            if (entity is Movie movie)
//            {
//                Console.WriteLine("This movie is " + movie.Name);
//            }
//
//            await new MediaListPopulating(connectionString).CreateImdbMovieList("ls051203792",
//                "\"The Great Movies\" by Roger Ebert");
//            await new MediaListPopulating(connectionString).CreateImdbMovieList("ls055315312",
//                "Top 95 Spy/Thriller Movies");
//
//            await new MediaListPopulating(connectionString).CreateImdbTVShowList("ls004499891",
//                "Best Of The Best TV Shows of 2000-2017");
//            
//            await new MediaListPopulating(connectionString).CreateImdbTVShowList("ls059270048",
//                "TV Series I Would Watch Again (2000-2014)"); 
//            
//            await new MediaListPopulating(connectionString).CreateImdbTVShowList("ls051600015",
//                "250: Top TV Series, HBO, Showtime:");

            }
        }
    }
}