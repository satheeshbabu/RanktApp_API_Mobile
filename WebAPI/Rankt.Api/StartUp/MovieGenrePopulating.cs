using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataModel.Base;
using DataModel.Genres;
using Newtonsoft.Json.Linq;
using Trakker.Api.Repositories.Genres.MovieGenres;
using TrakkerApp.Api.Repositories.Genres.MovieGenres;

namespace TrakkerApp.Api.StartUp
{
    public static class MovieGenrePopulating
    {
        public static async Task PopulateTmdbMovieGenres(MovieGenreRepository movieGenreRepository)
        {
            var genres = new List<MovieGenre>();

            try
            {
                const string myUrl = "https://api.themoviedb.org/3/genre/movie/list?api_key=661b76973b90b91e0df214904015fd4d";

                var client = new HttpClient();
                var data = await client.GetStringAsync(myUrl);

                var o = JObject.Parse(data);

                var genreJArray = (JArray)o["genres"];

                genres.AddRange(genreJArray.Select(item => MovieGenre.InstanciateTmdbGenreMovie(item["name"] + "", (int) item["id"])));
            }
            catch (Exception e)
            {
                Console.WriteLine("My error is " + e);
            }

            if (genres.Count > 0)
            {
                var tmdbGenres = await movieGenreRepository.GetAllGenresBySource(Category.SOURCE_TMDB);
                foreach (var movieGenre in genres)
                {
                    if (tmdbGenres== null || (!tmdbGenres.Any(c => c.Source.Equals(movieGenre.Source) && c.SourceId == movieGenre.SourceId)))
                    {
                        var error = await movieGenreRepository.Save(movieGenre);
                        if (error.ErrorCode == BaseError.Success)
                        {
                            Console.WriteLine("Created missing movie genre " + movieGenre.SourceName);
                        }
                        else
                        {
                            Console.WriteLine("An Error occured creating genre " + movieGenre.SourceName);
                        }
                    }
                }
            }
        }
    }
}