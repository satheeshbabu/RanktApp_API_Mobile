using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Base;
using DataModel.Genres;
using DataModel.Movies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Trakker.Api.Repositories.Genres.MovieGenres;
using Trakker.Api.Singletons;
using Trakker.Api.StartUp;
using TrakkerApp.Api.Repositories;
using TrakkerApp.Api.Repositories.Genres.MovieGenres;
using TrakkerApp.Api.Repositories.Relations;

namespace Trakker.Api.Repositories.Movies
{
    public class MovieRepository : BaseRepository<Movie>, IMovieRepository
    {
        private const string TABLE_NAME = "dbo.tblMovie";
        private const string ID_FIELD_NAME = "ID";
        private const string FIELD_NAME = "Name";
        private const string FIELD_OVERVIEW = "Overview";
        private const string FIELD_RELEASE_DATE = "Release_Date";
        private const string FIELD_RUN_TIME = "Run_Time";
        private const string FIELD_TMDB_ID = "TMDB_ID";
        private const string FIELD_IMDB_ID = "IMDB_ID";
        private const string FIELD_TMDB_POSTER_PATH = "TMDB_Poster_Path";
        private const string FIELD_TMDB_BACKDROP_PATH = "TMDB_Backdrop_Path";

        private const string CACHE_MOVIE_ID = "CACHE:MOVIE:ID";
        private const string CACHE_MOVIE_TMDB_ID = "CACHE:MOVIE:TMDBID";
        private const string CACHE_MOVIE_IMDB_ID = "CACHE:MOVIE:ID";
        private const string CACHE_MOVIE_RELATIONS = "CACHE:MOVIE:RELATIONS";
        private const string CACHE_MOVIE_GENRES = "CACHE:MOVIE:GENRES";

        private const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_OVERVIEW + ", " +
            TABLE_NAME + "." + FIELD_RELEASE_DATE + ", " +
            TABLE_NAME + "." + FIELD_RUN_TIME + ", " +
            TABLE_NAME + "." + FIELD_TMDB_ID + ", " +
            TABLE_NAME + "." + FIELD_IMDB_ID + ", " +
            TABLE_NAME + "." + FIELD_TMDB_POSTER_PATH + ", " +
            TABLE_NAME + "." + FIELD_TMDB_BACKDROP_PATH + " ";
       

        public MovieRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {

        }

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        private async Task<IEnumerable<Movie>> GetList(SqlConnection connection, string strSql)
        {
            var movies = new List<Movie>();
            try
            {
                await connection.OpenAsync();
                var command = new SqlCommand(strSql, connection);
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var movie = SerializeFromReader(reader);

                        await GetMovieGenres(movie);

                        movies.Add(movie);
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                connection.Close();
            }

            return movies;
        }

        private async Task GetMovieGenres(Movie movie)
        {
            var relationRepository = new RelationRepository(Configuration, MemoryCache);

            var isExist = MemoryCache.TryGetValue("CACHEMOVIERELATIONS" + movie.GetId(), out List<Relation> movieRelations);
            if (!isExist)
            {
                movieRelations = await relationRepository.GetRelationsByParent(
                    movie.GetEntityCategoryId(), movie.GetId());

                MemoryCache.Set("CACHEMOVIERELATIONS" + movie.GetId(), movieRelations);
                TrakkerCache.SaveCacheEntry("CACHEMOVIERELATIONS" + movie.GetId());
            }

            var movieGenreRepository = new MovieGenreRepository(Configuration, MemoryCache);

            foreach (var relation in movieRelations)
            {
                if (relation.CategoryTo == MovieGenre.ENTITY_CATEGORY_ID)
                {
                    movie.AddGenre(await movieGenreRepository.GetById(relation.EntityTo));
                }
            }
        }

        private static Movie SerializeFromReader(IDataRecord reader)
        {
            DateTime.TryParse(reader[FIELD_RELEASE_DATE].ToString(), out DateTime d2);

            var movie = Movie.CreateFromReader(
                long.Parse(reader[ID_FIELD_NAME].ToString()),
                reader[FIELD_NAME].ToString(),
                reader[FIELD_OVERVIEW].ToString(),
                d2,
                int.Parse(reader[FIELD_RUN_TIME].ToString()),
                long.Parse(reader[FIELD_TMDB_ID].ToString()),
                reader[FIELD_IMDB_ID].ToString(),
                reader[FIELD_TMDB_POSTER_PATH].ToString(),
                reader[FIELD_TMDB_BACKDROP_PATH].ToString());

            return movie;
        }

        public override async Task<Movie> GetById(long id)
        {
            var isExist = MemoryCache.TryGetValue("CACHEMOVIE" + id, out Movie movie);
            if (!isExist)
            {
                movie = await GetSingleByDesiredParameter(ID_FIELD_NAME, id);
                MemoryCache.Set("CACHEMOVIE" + id, movie);
                TrakkerCache.SaveCacheEntry("CACHEMOVIE" + id);
            }
            return movie;
        }

        public async Task<Movie> GetByTmdbId(long tmdbId)
        {
            return await GetSingleByDesiredParameter(FIELD_TMDB_ID, tmdbId);
        }

        public async Task<Movie> GetByImdbId(string imdbId)
        {
            return await GetSingleByDesiredParameter(FIELD_IMDB_ID, imdbId);
        }
        private async Task<Movie> GetSingleByDesiredParameter(string field, object passedInParameter)
        {
            var sqlQuery = GetBasicSelectSql(1) + " WHERE " +
                               TABLE_NAME + "." + field + " = ";
                
                if (field.Equals(FIELD_NAME) || field.Equals(FIELD_IMDB_ID))
                {
                    sqlQuery += "'" + passedInParameter + "'";
                }
                else if (field.Equals(ID_FIELD_NAME) || field.Equals(FIELD_TMDB_ID))
                {
                    sqlQuery += "'" + Convert.ToInt64(passedInParameter) + "'";
                }

                var movies = (await GetList(GetConnection(), sqlQuery)).ToList();

            return movies.Count > 0 ? movies[0] : null;
        }

        public async Task<List<Movie>> GetAllMovies()
        {
            var sqlQuery = GetBasicSelectSql(0);

            return (await GetList(GetConnection(), sqlQuery)).ToList();
        }

        public Task<IEnumerable<Movie>> GetAllMoviesByUserFromToken(string token)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Add(string token, Movie movie)
        {
            throw new System.NotImplementedException();
        }

        public Task<Movie> PutMovieAsync(string token, Movie movie)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteMovieAsync(string token, string tmdbid)
        {
            throw new System.NotImplementedException();
        }

        public override async Task<BaseError> Create(Movie entity)
        {
            var connection = GetConnection(); ;
            try
            {
                await connection.OpenAsync();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + FIELD_NAME +
                                         ", " + FIELD_OVERVIEW +
                                         ", " + FIELD_RELEASE_DATE +
                                         ", " + FIELD_RUN_TIME +
                                         ", " + FIELD_TMDB_ID +
                                         ", " + FIELD_IMDB_ID +
                                         ", " + FIELD_TMDB_POSTER_PATH +
                                         ", " + FIELD_TMDB_BACKDROP_PATH + ") OUTPUT INSERTED.ID VALUES " +
                                         "(@name, @overview, @releaseDate, @runTime, " +
                                         "@tmdbid, @imdbid, @tmdbPosterPath, @tmdbBackdropPath)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@name", entity.Name),
                    new SqlParameter("@overview", entity.Overview),
                    entity.ReleaseDate.HasValue
                        ? new SqlParameter("@releaseDate", entity.ReleaseDate)
                        : new SqlParameter("@releaseDate", DBNull.Value),
                    new SqlParameter("@runTime", entity.RunTime),
                    new SqlParameter("@tmdbid", entity.TmdbId),
                    new SqlParameter("@imdbid", entity.ImdbId),
                    new SqlParameter("@tmdbPosterPath", entity.TmdbPosterPath),
                    new SqlParameter("@tmdbBackdropPath", entity.TmdbBackdropPath)
                };

                var command = new SqlCommand(insertSql, connection);

                foreach (var sqlParameter in parameters)
                {
                    command.Parameters.Add(sqlParameter);
                }
                entity.SetId(Convert.ToInt32(await command.ExecuteScalarAsync()));
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                connection.Close();
            }
            return entity.GetId() == 0 ? new BaseError(BaseError.Fail) : new BaseError(BaseError.Success);
        }

        public override Task<BaseError> Update(Movie entity)
        {
            throw new NotImplementedException();
        }
    }
}