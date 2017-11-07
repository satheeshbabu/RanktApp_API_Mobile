using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using DataModel.Genres;
using DataModel.TVShows;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using TrakkerApp.Api.Repositories.Genres;
using TrakkerApp.Api.Repositories.Genres.MovieGenres;
using TrakkerApp.Api.Repositories.Genres.TVShowGenres;
using TrakkerApp.Api.Repositories.Relations;
using TrakkerApp.Api.Repositories.TVShows;

namespace TrakkerApp.Api.StartUp
{
    public class TVShowPopulating
    {
        private static ITVShowRepository _tvShowRepository;
        private static IRelationRepository _relationRepository;
        private static ITVShowGenreRepository _tvShowGenreRepository;
        private static List<TVShowGenre> _tmdbTVShowGenres;

        public TVShowPopulating(IConfiguration configuration, IMemoryCache cache)
        {
            _tvShowRepository = new TVShowRepository(configuration);
            _relationRepository = new RelationRepository(configuration, cache);
            _tvShowGenreRepository = new TVShowGenreRepository(configuration);
        }
        
        public async Task AddTVShow(int movieId)
        {
            await PopulateListOfTmdbGenres();

            await AddTVShowToDatabase(movieId);
        }
        
        public async Task PopulateManyTVShowsTask()
        {
            await PopulateListOfTmdbGenres();

            const int numPages = 3;
            for (var i = 1; i <= numPages; i++)
            {
                await AddPageOfPopularTVShows(i);
            }
            Console.WriteLine("I Have added " + numPages + " pages of TV Shows");
        }
        
        private async Task PopulateListOfTmdbGenres()
        {
            _tmdbTVShowGenres = await _tvShowGenreRepository.GetAllGenresBySource(Category.SOURCE_TMDB);
        }
        
        private async Task AddPageOfPopularTVShows(int page)
        {
            try
            {
                var myURL = "https://api.themoviedb.org/3/discover" +
                            "/tv?api_key=661b76973b90b91e0df214904015fd4d&page=" + page;
                var client = new HttpClient();
                var data = await client.GetStringAsync(myURL);

                var o = JObject.Parse(data);

                var results = (JArray)o["results"];

                foreach (var result in results)
                {
                    if (await _tvShowRepository.GetByTmdbId((int)result["id"]) != null) continue;
                    await AddTVShowToDatabase((int)result["id"]);
                    await Task.Delay(200);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Adding Page of TV Shows: My error is " + e);
            }
        }
        
        private static async Task AddTVShowToDatabase(int tvShowId)
        {
            try
            {
                var myUrl = "https://api.themoviedb.org/3/tv/" + tvShowId +
                            "?api_key=661b76973b90b91e0df214904015fd4d";
                var client = new HttpClient();
                var data = await client.GetStringAsync(myUrl);

                var o = JObject.Parse(data);
                var episodeRunTimeArray = (JArray) o["episode_run_time"];

                var episodeRunTime = episodeRunTimeArray[0] + "";
               
                var tvShow = TVShow.Instanciate(o["original_name"] + "",
                    o["overview"] + "",
                    string.IsNullOrEmpty(o["first_air_date"] + "") ? (DateTime?)null : DateTime.Parse(o["first_air_date"] + ""),
                    string.IsNullOrEmpty(episodeRunTime) ? 0 : Int32.Parse(episodeRunTime),
                    string.IsNullOrEmpty(o["id"] + "") ? 0 : Int32.Parse(o["id"] + ""),
                    o["imdb_id"] + "",
                    o["poster_path"] + "",
                    o["backdrop_path"] + "");

                await _tvShowRepository.Save(tvShow);

                var genreArray = (JArray)o["genres"];

                foreach (var item in genreArray)
                {
                    //Checked to ensure is TMDB when list is loaded from memory
                    var genre = _tmdbTVShowGenres.FirstOrDefault(x => x.SourceId == (int)item["id"] );
                    if (genre != null)
                    {
                        await _relationRepository.CreateTVShowToTVShowGenreRelationship(tvShow, genre);
                    }
                }

                int year;
                
                year = tvShow.FirstAirDate?.Year ?? 1900;

                Console.WriteLine(tvShow.GetId().ToString("0000") + " : " + tvShow.Name + " released in " +
                                  year + " with IMDB id " + o["imdb_id"] + " " + year);
                
                await AddExternalIds(tvShow);

                await _tvShowRepository.Save(tvShow);

                Console.WriteLine("Added external ids for " + tvShow.Name + " imdb " + tvShow.ImdbId);

            }
            catch (Exception e)

            {
                Console.WriteLine("Adding TV Show to DB: My error is " + e);
            }
        }

        private static async Task AddExternalIds(TVShow tvShow)
        {
            try
            {
                var myUrl = "https://api.themoviedb.org/3/tv/" + tvShow.TmdbId +
                            "/external_ids?api_key=661b76973b90b91e0df214904015fd4d";
                var client = new HttpClient();
                var data = await client.GetStringAsync(myUrl);

                var o = JObject.Parse(data);

                tvShow.ImdbId = o["imdb_id"] + "";
                tvShow.TvdbId = (int) o["tvdb_id"];
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Adding External Ids: My error is " + e);
            }
        }
    }
}