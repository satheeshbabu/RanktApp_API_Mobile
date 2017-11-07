using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataModel.Base;
using DataModel.Genres;
using Newtonsoft.Json.Linq;
using TrakkerApp.Api.Repositories.Genres.TVShowGenres;

namespace TrakkerApp.Api.StartUp
{
    public static class TVShowGenrePopulating
    {
        public static async Task PopulateTmdbTVShowGenres(TVShowGenreRepository tvGenreRepository)
        {
            var genres = new List<TVShowGenre>();

            try
            {
                const string myUrl = "https://api.themoviedb.org/3/genre/tv/list?api_key=661b76973b90b91e0df214904015fd4d";

                var client = new HttpClient();
                var data = await client.GetStringAsync(myUrl);

                var o = JObject.Parse(data);

                var genreJArray = (JArray)o["genres"];

                genres.AddRange(genreJArray.Select(item => TVShowGenre.InstanciateTmdbGenreTVShow(
                            item["name"] + "", (int) item["id"])));
            }
            catch (Exception e)
            {
                Console.WriteLine("My error is " + e);
            }

            if (genres.Count > 0)
            {
                var tmdbGenres = await tvGenreRepository.GetAllGenresBySource(Category.SOURCE_TMDB);
                foreach (var tvShowGenre in genres)
                {
                    if (tmdbGenres == null || (!tmdbGenres.Any(c => c.Source.Equals(tvShowGenre.Source) && c.SourceId == tvShowGenre.SourceId)))
                    {
                        var error = await tvGenreRepository.Save(tvShowGenre);
                        if (error.ErrorCode == BaseError.Success)
                        {
                            Console.WriteLine("Created missing movie genre " + tvShowGenre.SourceName);
                        }
                        else
                        {
                            Console.WriteLine("An Error occured creating genre " + tvShowGenre.SourceName);
                        }
                    }
                }
            }
        }
    }
}