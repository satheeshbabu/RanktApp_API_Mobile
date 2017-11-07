using System.Threading.Tasks;
using DataModel.Movies;
using TrakkerApp.Api.Repositories;

namespace Trakker.Api.Repositories.Movies
{
    public interface IMovieCollectionRepository : IBaseRepository<MovieCollection>
    {
        Task<MovieCollection> GetBySourceAndMovieCollectionId(long source, long id);
    }
}