using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Movies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Rankt.Api.Controllers;
using Trakker.Api.Repositories.Movies;
using Trakker.Api.Singletons;
using Trakker.Api.StartUp;
using TrakkerApp.Api.Controllers.HelperClasses;

namespace Trakker.Api.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SettingController: Controller
    {
        private readonly IMovieRepository _repository;

        private readonly IStringLocalizer<MovieController> _localizer;
        private readonly IMemoryCache _memoryCache;

        public SettingController(IMovieRepository repository, IStringLocalizer<MovieController> localizer, IMemoryCache memoryCache)
        {
            _repository = repository;
            _localizer = localizer;
            _memoryCache = memoryCache;
        }

        // GET: api/values
        [HttpGet]
        public ActionResult Get()
        {
            TrakkerCache.ClearCache(_memoryCache);
            return Json("Hello");
        }

        // GET api/values/5
//            [HttpGet("{id}")]
//            public async Task<ActionResult> Get(long id)
//            {
//                
//            }

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

