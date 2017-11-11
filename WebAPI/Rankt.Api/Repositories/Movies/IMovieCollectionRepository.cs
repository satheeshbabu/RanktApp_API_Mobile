using System.Threading.Tasks;
using DataModel.Movies;

namespace Rankt.Api.Repositories.Movies
{
    public interface IMovieCollectionRepository : IBaseRepository<MovieCollection>
    {
        Task<MovieCollection> GetBySourceAndMovieCollectionId(long source, long id);
    }
}