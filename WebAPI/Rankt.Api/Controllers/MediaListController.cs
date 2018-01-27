using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Rankt.Api.Repositories.Lists;
using Rankt.Api.Repositories.Movies;
using TrakkerApp.Api.Controllers.HelperClasses;
using TrakkerApp.Api.Repositories.Relations;

namespace Rankt.Api.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MediaListController : Controller
    {
        private readonly IMediaListRepository _repository;
        private readonly IRelationRepository _relationRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IStringLocalizer<SharedResources> _localizer;

        private const string CacheMovie = "CACHE:MOVIE";

        public MediaListController(IMediaListRepository repository,
            IRelationRepository relationRepository,
            IMovieRepository movieRepository,
            IMemoryCache memoryCache, 
            IStringLocalizer<SharedResources> localizer)
        {
            _repository = repository;
            _relationRepository = relationRepository;
            _movieRepository = movieRepository;
            _memoryCache = memoryCache;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] QueryPagenationParameters pageParameters)
        {
            //TODO pass in if want elements
            var mediaLists = await _repository.GetAllLists(true);
            
            if (mediaLists == null || mediaLists.Count == 0)
            {
                string message = _localizer["controllers.medialist.error.no_media_lists_found"];
                var token = new JObject { { "message", message } };

                var errorContent = Content(token.ToString(), "application/json");
                errorContent.StatusCode = (int)HttpStatusCode.NotFound;
                return errorContent;
            }
            
            var array = new JArray();
            foreach (var mediaList in mediaLists)
            {
                array.Add(mediaList.ToJsonToken());
            }

            var jObject = new JObject
            {
                {"number_media_lists", mediaLists.Count},
                { "media_lists", array},
                {"tempt", _localizer["controllers.medialist.error.no_media_lists_found"].ToString() }
            };

            Console.WriteLine("Returning List of " + mediaLists.Count+ " media Lists");
            var content = Content(jObject.ToString(), "application/json");
            return content;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(long id)
        {
            //            var mediaList = await _repository.GetById(id);
            //            
            //            if (mediaList == null)
            //            {
            //                string message = _localizer["noMediaList"];
            //                var errorToken = new JObject{{ "message", message}};
            //
            //                var errorContent = Content(errorToken.ToString(), "application/json");
            //                errorContent.StatusCode = (int)HttpStatusCode.NotFound;
            //                return errorContent;
            //            }
            //
            //            var token = mediaList.ToJsonToken();
            //            
            //            var content = Content(token.ToString(), "application/json");
            //            content.StatusCode = (int) HttpStatusCode.OK;
            //            return content;

            var movie = await _movieRepository.GetById(id);

            if (movie == null)
            {
                string message = _localizer["movieNotFound"];
                var token = new JObject { { "message", message } };

                var errorContent = Content(token.ToString(), "application/json");
                errorContent.StatusCode = (int)HttpStatusCode.NotFound;
                return errorContent;
            }
            //                _memoryCache.Set(CacheMovie + id, movie, TimeSpan.FromHours(6));
            //            }

            var content = Content(movie.ToJsonToken().ToString(), "application/json");
            return content;
        }
    }
}