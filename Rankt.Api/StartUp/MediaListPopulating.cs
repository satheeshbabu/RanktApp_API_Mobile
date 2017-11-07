using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using DataModel.Movies;
using DataModel.Overall;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Trakker.Api.Repositories.Movies;
using TrakkerApp.Api.Parsers.Imdb;
using TrakkerApp.Api.Repositories.Genres.MovieGenres;
using TrakkerApp.Api.Repositories.Lists;
using TrakkerApp.Api.Repositories.Relations;
using TrakkerApp.Api.Repositories.TVShows;

namespace TrakkerApp.Api.StartUp
{
    public class MediaListPopulating
    {
        private readonly MovieRepository _movieRepository;
        private readonly TVShowRepository _tvShowRepository;
        private readonly RelationRepository _relationRepository;
        private readonly MediaListRepository _mediaListRepository;
        private MediaList _mediaList;

        private IConfiguration _configuration;
        private IMemoryCache _cache;

        public MediaListPopulating(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
            _movieRepository = new MovieRepository(configuration,null);
            _relationRepository = new RelationRepository(configuration, _cache);
            _mediaListRepository = new MediaListRepository(configuration);
            _tvShowRepository = new TVShowRepository(configuration);
        }

        public async Task CreateImdbMovieList(string listId, string listTitle)
        {
            var movieListParser =
                new MediaListParser(listId, listTitle);
            await movieListParser.GetListOfMedia();

            _mediaList = await _mediaListRepository.GetBySourceAndCollectionId(Category.SOURCE_IMDB, listId, true);

            if (_mediaList != null)
            {
                //TODO If list exists, want size to compare with size of IMDB list 
                //and then update if necessary

                Console.WriteLine("List with id {0} and title {1} already exists", listId, listTitle);
                return;
            }
            _mediaList = MediaList.InstanciateImdbMovieList(movieListParser.ListId, movieListParser.ListName
                , movieListParser.ListDescription, null, null);

            await _mediaListRepository.Save(_mediaList);

            var moviePopulating = new MoviePopulating(_configuration, _cache);

            foreach (var movieImdbId in movieListParser.MediaList)
            {
                try
                {
                    await Task.Delay(200);
                    var movie = await _movieRepository.GetByImdbId(movieImdbId);
                    if (movie == null)
                    {
                        var myUrl = "https://api.themoviedb.org/3/find/" + movieImdbId +
                                    "?api_key=661b76973b90b91e0df214904015fd4d&external_source=imdb_id";
                        var client = new HttpClient();
                        var data = await client.GetStringAsync(myUrl);

                        var o = JObject.Parse(data);
                        var a = (JArray) o["movie_results"];

                        var tmdbId = (int) a[0]["id"];

                        await moviePopulating.AddMovie(tmdbId);
                        await Task.Delay(200);
                        movie = await _movieRepository.GetByTmdbId(tmdbId);
                    }
                    await _relationRepository.CreateMediaListToMovieRelationship(_mediaList, movie);
                }
                catch (Exception e)

                {
                    Console.WriteLine("My error is " + e);
                }
            }
        }
        
        public async Task CreateImdbTVShowList(string listId, string listTitle)
        {
            var tvShowListParser =
                new MediaListParser(listId, listTitle);
            await tvShowListParser.GetListOfMedia();

            _mediaList = await _mediaListRepository.GetBySourceAndCollectionId(Category.SOURCE_IMDB, listId, true);

            if (_mediaList != null)
            {
                //TODO If list exists, want size to compare with size of IMDB list 
                //and then update if necessary

                Console.WriteLine("List with id {0} and title {1} already exists", listId, listTitle);
                return;
            }
            _mediaList = MediaList.InstanciateImdbTVShowList(tvShowListParser.ListId, tvShowListParser.ListName
                , tvShowListParser.ListDescription, null, null);

            await _mediaListRepository.Save(_mediaList);

            var tvShowPopulating = new TVShowPopulating(_configuration, _cache);

            foreach (var tvShowImdbId in tvShowListParser.MediaList)
            {
                try
                {
                    await Task.Delay(200);
                    var tvShow = await _tvShowRepository.GetByImdbId(tvShowImdbId);
                    if (tvShow == null)
                    {
                        var myUrl = "https://api.themoviedb.org/3/find/" + tvShowImdbId +
                                    "?api_key=661b76973b90b91e0df214904015fd4d&external_source=imdb_id";
                        var client = new HttpClient();
                        var data = await client.GetStringAsync(myUrl);

                        var o = JObject.Parse(data);
                        var a = (JArray) o["tv_results"];

                        var tmdbId = (int) a[0]["id"];

                        await tvShowPopulating.AddTVShow(tmdbId);
                        await Task.Delay(200);
                        tvShow = await _tvShowRepository.GetByTmdbId(tmdbId);
                        
                    }
                    await _relationRepository.CreateMediaListToTVShowRelationship(_mediaList, tvShow);
                    Console.WriteLine("Show " + tvShow.Name + " added to list " + _mediaList.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine("My error is " + e);
                }
            }
        }
    }
}