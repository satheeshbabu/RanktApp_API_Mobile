using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Movies;
using DataModel.Movies;
using TrakkerApp.Api.Repositories;

namespace Trakker.Api.Repositories.Movies
{
    public interface IMovieRepository : IBaseRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetAllMoviesByUserFromToken(string token);
        Task<bool> Add(string token, Movie movie);
        Task<Movie> PutMovieAsync(string token, Movie movie);
        Task<bool> DeleteMovieAsync(string token, string tmdbid);
        Task<Movie> GetByImdbId(string imdbId);
        Task<Movie> GetByTmdbId(long tmdbId);
        Task<List<Movie>> GetAllMovies();
    }
}