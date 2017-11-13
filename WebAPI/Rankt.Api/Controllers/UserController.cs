using System;
using System.Net;
using System.Threading.Tasks;
using Common.Model.Users;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Rankt.Api.Repositories.Users;
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
    public class UserController : Controller
    {
        private readonly IUserRepository _repository;

        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IMemoryCache _memoryCache;

        private const string CacheMovie = "CACHE:MOVIE";

        public UserController(IUserRepository repository, IStringLocalizer<SharedResources> localizer, IMemoryCache memoryCache)
        {
            _repository = repository;
            _localizer = localizer;
            _memoryCache = memoryCache;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateUser createUser)
        //string userName, string password, string emailAddress)
        {
            //TODO ensure user doesn't exist
            var user = Common.Model.Users.User.Instanciate(createUser.Username, createUser.Password,
                createUser.EmailAddress, DateTime.UtcNow, DateTime.UtcNow, false, null);

            await _repository.Save(user);

            var content = Content(user.ToJsonToken().ToString(), "application/json");
            return content;
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
