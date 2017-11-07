using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Base;
using DataModel.Genres;
using DataModel.Movies;
using DataModel.Overall;
using DataModel.TVShows;

namespace TrakkerApp.Api.Repositories.Relations
{
    public interface IRelationRepository
    {
        Task CreateMediaListToMovieRelationship(MediaList mediaList, Movie movie);
        Task CreateMediaListToTVShowRelationship(MediaList mediaList, TVShow tvShow);
        Task CreateMovieCollectionToMovieRelationship(MovieCollection movieCollection, Movie movie);
        Task CreateMovieToMovieGenreRelationship(Movie movie, MovieGenre movieGenre);
        Task CreateTVShowToTVShowGenreRelationship(TVShow tvShow, TVShowGenre tvShowGenre);
        Task<List<Relation>> GetRelationsByParent(long parentCatId, long parentEntityId);

    }
}