using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using DataModel.Movies;
using DataModel.TVShows;
using Trakker.Api.Repositories;
using Trakker.Api.Repositories.Movies;
using TrakkerApp.Api.Repositories;
using TrakkerApp.Api.Repositories.TVShows;

namespace TrakkerApp.Api.StartUp
{
    public static class BaseModelResolver
    {
//        private static IConfiguration _configuration { get; set; }
//
//        public static void SetConfiguration(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }
//
//        public static Dictionary<long, IBaseRepository<BaseEntity>> Repositories = 
//                new Dictionary<long, IBaseRepository<BaseEntity>>();
//        
//        public static async Task<BaseEntity> ResolveBaseEntity(long catId, long entityId)
//        {
//            if (catId == Movie.ENTITY_CATEGORY_ID)
//            {
//                var repo = new MovieRepository(_configuration, null);
//                return await repo.GetById(entityId);
//            }
//            if (catId == TVShow.ENTITY_CATEGORY_ID)
//            {
//                var repo = new TVShowRepository(_configuration);
//                return await repo.GetById(entityId);
//            }
//
//            return null;
//        }
    }
}