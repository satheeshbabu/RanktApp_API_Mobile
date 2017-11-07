using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Genres;
using Trakker.Api.Repositories;

namespace TrakkerApp.Api.Repositories.Genres.MovieGenres
{
    public interface IMovieGenreRepository : IBaseRepository<MovieGenre>
    {
        Task<List<MovieGenre>> GetAllGenresBySource(long source);
    }
}