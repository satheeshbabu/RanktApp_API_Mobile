using System;
using System.Net;
using System.Threading.Tasks;
using Common.Model.Elo;
using DataModel.Base;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Rankt.Api.Repositories.Elo;
using Rankt.Api.Repositories.Movies;
using Trakker.Api.Repositories.Movies;
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
    public class EloController : Controller
    {
        private readonly IEloListRepository _repository;

        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IMemoryCache _memoryCache;

        private const string CacheMovie = "CACHE:MOVIE";

        public EloController(IEloListRepository repository, IStringLocalizer<SharedResources> localizer, IMemoryCache memoryCache)
        {
            _repository = repository;
            _localizer = localizer;
            _memoryCache = memoryCache;
        }

        // GET: api/values
        [HttpGet]
        public async Task<ActionResult> GetAllListsForUser([FromQuery]QueryPagenationParameters pageParameters)
        {
//            var movies = await _repository.GetAllMovies();
//
//            if (movies == null || movies.Count == 0)
//            {
//                string message = _localizer["controllers.movie.movie_not_found"];
//                var token = new JObject { { "message", message } };
//
//                var errorContent = Content(token.ToString(), "application/json");
//                errorContent.StatusCode = (int)HttpStatusCode.NotFound;
//                return errorContent;
//            }
//
//            var array = new JArray();
//            foreach (var movie in movies)
//            {
//                array.Add(movie.ToJsonToken());
//            }
//
//            var jObject = new JObject
//            {
//                {"number_movies", movies.Count},
//                { "movies", array}
//            };
//
//            Console.WriteLine("Returning List of " + movies.Count+ " movies");
//            var content = Content(jObject.ToString(), "application/json");
//            return content;
            return Json("ok");
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
                    string message = _localizer["movieNotFound"];
                    var token = new JObject {{"message", message}};

                    var errorContent = Content(token.ToString(), "application/json");
                    errorContent.StatusCode = (int) HttpStatusCode.NotFound;
                    return errorContent;
                }
//                _memoryCache.Set(CacheMovie + id, movie, TimeSpan.FromHours(6));
//            }

            var content = Content(movie.ToJsonToken().ToString(), "application/json");
            return content;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]CreateEloList createEloList)
        {
            //TODO ensure list name doesnt exists
            //TODO get user from passed in TOKEN

            var currentDateTime = DateTime.UtcNow;

            var eloList = EloList.Instanciate(createEloList.Name, createEloList.UserId, currentDateTime,
                currentDateTime);
            //TODO how to wrap save calls
            var error = await _repository.Save(eloList);

            if (error.ErrorCode == BaseError.Success)
            {
                var content = Content(eloList.ToJsonToken().ToString(), "application/json");
                return content;
            }
            
            return Json("error");
            
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
