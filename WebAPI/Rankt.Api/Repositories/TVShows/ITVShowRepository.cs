using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.TVShows;
using Rankt.Api.Repositories;
using Trakker.Api.Repositories;

namespace TrakkerApp.Api.Repositories.TVShows
{
    public interface ITVShowRepository : IBaseRepository<TVShow>
    {
        Task<TVShow> GetByImdbId(string imdbId);
        Task<TVShow> GetByTmdbId(long tmdbId);
        Task<TVShow> GetSingleByDesiredParameter(string field, object passedInParameter);
        Task<List<TVShow>> GetAllTVShows();
    }
}