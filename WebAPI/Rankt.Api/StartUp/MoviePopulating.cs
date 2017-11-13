using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Model.Movies;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using DataModel.Genres;
using DataModel.Movies;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Rankt.Api.Repositories.Movies;
using Trakker.Api.Repositories.Genres.MovieGenres;
using Trakker.Api.Repositories.Movies;
using TrakkerApp.Api.Repositories.Genres;
using TrakkerApp.Api.Repositories.Genres.MovieGenres;
using TrakkerApp.Api.Repositories.Relations;

namespace TrakkerApp.Api.StartUp
{
    public class MoviePopulating
    {
        private static IMovieRepository _movieRepository;
        private static IRelationRepository _relationRepository;
        private static IMovieGenreRepository _movieGenreRepository;
        private static IMovieCollectionRepository _moviecollectionRepository;
        private static List<MovieGenre> _tmdbMovieGenres;

        public MoviePopulating(IConfiguration configuration, IMemoryCache cache)
        {
            _movieRepository = new MovieRepository(configuration, cache);
            _relationRepository = new RelationRepository(configuration, cache);
            _movieGenreRepository = new MovieGenreRepository(configuration, cache);
            _moviecollectionRepository = new MovieCollectionRepository(configuration, cache);
        }

        public async Task AddMovie(int movieId)
        {
            await PopulateListOfTmdbGenres();

            await AddMovieToDatabase(movieId);
        }

        public async Task PopulateManyMovies()
        {
            await PopulateListOfTmdbGenres();

            const int numPages = 5;
            for (var i = 1; i <= numPages; i++)
            {
                await AddPageOfPopularMovies(i);
            }
            Console.WriteLine("I Have added " + numPages + " pages of movies");
        }

        private static async Task PopulateListOfTmdbGenres()
        {
            _tmdbMovieGenres = await _movieGenreRepository.GetAllGenresBySource(Category.SOURCE_TMDB);
        }

        private static async Task AddPageOfPopularMovies(int page)
        {
            try
            {
                var myUrl = "https://api.themoviedb.org/3/discover" +
                            "/movie?api_key=661b76973b90b91e0df214904015fd4d&page=" + page;
                var client = new HttpClient();
                var data = await client.GetStringAsync(myUrl);

                var o = JObject.Parse(data);

                var results = (JArray)o["results"];

                foreach (var result in results)
                {
                    if (await _movieRepository.GetByTmdbId((int)result["id"]) != null) continue;
                    await AddMovieToDatabase((int)result["id"]);
                    await Task.Delay(200);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("My error is " + e);
            }
        }

        private static async Task AddMovieToDatabase(int movieId)
        {
            try
            {
                var myUrl = "https://api.themoviedb.org/3/movie/" + movieId +
                            "?api_key=661b76973b90b91e0df214904015fd4d";
                var client = new HttpClient();
                var data = await client.GetStringAsync(myUrl);

                var o = JObject.Parse(data);
                
                var movie = Movie.Instanciate(o["title"] + "",
                    o["overview"] + "",
                    string.IsNullOrEmpty(o["release_date"] + "") ? (DateTime?)null : DateTime.Parse(o["release_date"] + ""),
                    string.IsNullOrEmpty(o["runtime"]+"") ? 0 : Int32.Parse(o["runtime"]+""),
                    string.IsNullOrEmpty(o["id"]+"") ? 0 : Int32.Parse(o["id"]+""),
                    o["imdb_id"] + "",
                    o["poster_path"] + "",
                    o["backdrop_path"] + "",
                    DateTime.UtcNow);

                await _movieRepository.Save(movie);

                var genreArray = (JArray)o["genres"];

                foreach (var item in genreArray)
                {
                    //Checked to ensure is TMDB when list is loaded from memory
                    var genre = _tmdbMovieGenres.First(x => x.SourceId == (int)item["id"] );
                    await _relationRepository.CreateMovieToMovieGenreRelationship(movie, genre);
                }

                var collectiontoken = o["belongs_to_collection"];

                var isPartOfCollection = "";

                if (collectiontoken.HasValues)
                {
                    isPartOfCollection = " Is part of collection " + (int)collectiontoken["id"];
                    await AddMovieCollection((int)collectiontoken["id"]);
                    await Task.Delay(200);
                }

                int year;
                if (movie.ReleaseDate.HasValue)
                {
                    year = movie.ReleaseDate.Value.Year;
                }
                else
                {
                    year = 1900;
                }

                Console.WriteLine(movie.GetId().ToString("0000") + " : " + movie.Name + " released in " +
                                  year + " with IMDB id " + o["imdb_id"] + " " + isPartOfCollection);

            }
            catch (Exception e)

            {
                Console.WriteLine("My error is " + e);
            }
        }

        private static async Task AddMovieCollection(int collectionId)
        {
            if (await _moviecollectionRepository.GetBySourceAndMovieCollectionId
                    (Category.SOURCE_TMDB, collectionId) == null)
            {
                try
                {
                    var myUrl = "https://api.themoviedb.org/3/collection/" + collectionId +
                                "?api_key=661b76973b90b91e0df214904015fd4d";
                    var client = new HttpClient();
                    var data = await client.GetStringAsync(myUrl);

                    var o = JObject.Parse(data);

                    var movieCollection = MovieCollection.InstanciateTmdbCollection(
                        (int)o["id"], o["name"] + "", o["overview"] + "",
                        o["poster_path"] + "", o["backdrop_path"] + "");

                    await _moviecollectionRepository.Save(movieCollection);

                    var parts = (JArray)o["parts"];

                    foreach (var token in parts)
                    {
                        var movie = await _movieRepository.GetByTmdbId((int)token["id"]);
                        if (movie == null)
                        {
                            await AddMovieToDatabase((int)token["id"]);
                            await Task.Delay(200);
                            movie = await _movieRepository.GetByTmdbId((int)token["id"]);
                        }

                        await _relationRepository.CreateMovieCollectionToMovieRelationship(movieCollection, movie);
                    }

                    Console.WriteLine(movieCollection.Name + " Collection added with " + parts.Count + " movies added");
                }
                catch (Exception e)
                {
                    Console.WriteLine("My error is " + e);
                }
            }
        }

    }
}