using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Genres;
using Rankt.Api.Repositories;
using Trakker.Api.Repositories;

namespace TrakkerApp.Api.Repositories.Genres.TVShowGenres
{
    public interface ITVShowGenreRepository : IBaseRepository<TVShowGenre>
    {
        Task<List<TVShowGenre>> GetAllGenresBySource(long source);
    }
}