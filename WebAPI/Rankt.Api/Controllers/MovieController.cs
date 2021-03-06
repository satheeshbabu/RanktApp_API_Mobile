﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Rankt.Api.Controllers.HelperClasses;
using Rankt.Api.Repositories.Movies;
using TrakkerApp.Api.Controllers.HelperClasses;

// For more information on enabling Web API for empty projects,
//visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rankt.Api.Controllers
{
    //TODO add this for culture, manage in React App
    ///?culture=fr-FR

    [EnableCors("SiteCorsPolicy")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository _repository;

        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IMemoryCache _memoryCache;

        private const string CacheMovie = "CACHE:MOVIE";

        public MovieController(IMovieRepository repository, IStringLocalizer<SharedResources> localizer, IMemoryCache memoryCache)
        {
            _repository = repository;
            _localizer = localizer;
            _memoryCache = memoryCache;
        }

        // GET: api/values
        [HttpGet]
        public async Task<ActionResult> GetAllMovies([FromQuery]QueryPagenationParameters pageParameters)
        {
            var movies = await _repository.GetAllMovies();

            if (movies == null || movies.Count == 0)
            {
                string message = _localizer["controllers.movie.error.no_movies_found"];
                return ResponseGenerator.NotFoundResult(message);
            }

            var array = new JArray();
            foreach (var movie in movies)
            {
                array.Add(movie.ToJsonToken());
            }

            var jObject = new JObject
            {
                {"number_movies", movies.Count},
                { "movies", array}
            };

            Console.WriteLine("Returning List of " + movies.Count+ " movies");
            return ResponseGenerator.OkResult(jObject);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(long id)
        {
//            var isExist = _memoryCache.TryGetValue(CacheMovie + id, out Movie movie);
//            if (!isExist)
//            {

                var movie = await _repository.GetById(id);

                if (movie == null)
                {
                    //TODO get message key from resourses
                    string message = _localizer["controllers.movie.error.movie_not_found"];
                    return ResponseGenerator.NotFoundResult(message);
                }
//                _memoryCache.Set(CacheMovie + id, movie, TimeSpan.FromHours(6));
//            }
            return ResponseGenerator.OkResult(movie);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpGet("delete/{id}")]
        public void Trstwww(int id)
        {
            

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
