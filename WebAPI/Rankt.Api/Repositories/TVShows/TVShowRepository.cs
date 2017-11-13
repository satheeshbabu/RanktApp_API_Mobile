using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using DataModel.TVShows;
using Microsoft.Extensions.Caching.Memory;
using Rankt.Api.Repositories;
using Trakker.Api.Repositories;

namespace TrakkerApp.Api.Repositories.TVShows
{
    public class TVShowRepository : BaseRepository<TVShow>, ITVShowRepository
    {
        private const string TABLE_NAME = "dbo.tblTvShow";
        private const string ID_FIELD_NAME = "ID";
        private const string FIELD_NAME = "Name";
        private const string FIELD_OVERVIEW = "Overview";
        private const string FIELD_FIRST_AIR_DATE = "First_Air_Date";
        private const string FIELD_EPISODE_RUN_TIME = "Episode_Run_Time";
        private const string FIELD_TMDB_ID = "TMDB_ID";
        private const string FIELD_IMDB_ID = "IMDB_ID";
        private const string FIELD_TVDB_ID = "TVDB_ID";
        private const string FIELD_TMDB_POSTER_PATH = "TMDB_Poster_Path";
        private const string FIELD_TMDB_BACKDROP_PATH = "TMDB_Backdrop_Path";

        private const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_OVERVIEW + ", " +
            TABLE_NAME + "." + FIELD_FIRST_AIR_DATE + ", " +
            TABLE_NAME + "." + FIELD_EPISODE_RUN_TIME + ", " +
            TABLE_NAME + "." + FIELD_TMDB_ID + ", " +
            TABLE_NAME + "." + FIELD_IMDB_ID + ", " +
            TABLE_NAME + "." + FIELD_TVDB_ID + ", " +
            TABLE_NAME + "." + FIELD_TMDB_POSTER_PATH + ", " +
            TABLE_NAME + "." + FIELD_TMDB_BACKDROP_PATH + " ";

        public TVShowRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {

        }

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }
        
        private static async Task<IEnumerable<TVShow>> GetList(SqlConnection connection, string strSql)
        {
            var movies = new List<TVShow>();
            try
            {
                await connection.OpenAsync();
                var command = new SqlCommand(strSql, connection);
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        movies.Add(SerializeFromReader(reader));
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
        
        private static TVShow SerializeFromReader(IDataRecord reader)
        {
            DateTime.TryParse(reader[FIELD_FIRST_AIR_DATE].ToString(), out DateTime d2);

            var tvShow = TVShow.CreateFromReader(
                long.Parse(reader[ID_FIELD_NAME].ToString()),
                reader[FIELD_NAME].ToString(),
                reader[FIELD_OVERVIEW].ToString(),
                d2,
                int.Parse(reader[FIELD_EPISODE_RUN_TIME].ToString()),
                long.Parse(reader[FIELD_TMDB_ID].ToString()),
                reader[FIELD_IMDB_ID].ToString(),
                long.Parse(reader[FIELD_TVDB_ID].ToString()),
                reader[FIELD_TMDB_POSTER_PATH].ToString(),
                reader[FIELD_TMDB_BACKDROP_PATH].ToString());

            return tvShow;
        }

        public override async Task<TVShow> GetById(long id)
        {
            return await GetSingleByDesiredParameter(ID_FIELD_NAME, id);
        }

        public async Task<TVShow> GetByImdbId(string imdbId)
        {
            return await GetSingleByDesiredParameter(FIELD_IMDB_ID, imdbId);
        }

        public async Task<TVShow> GetByTmdbId(long tmdbId)
        {
            return await GetSingleByDesiredParameter(FIELD_TMDB_ID, tmdbId);
        }

        public async Task<TVShow> GetSingleByDesiredParameter(string field, object passedInParameter)
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

            var tvShows = (await GetList(GetConnection(), sqlQuery)).ToList();

            return tvShows.Count > 0 ? tvShows[0] : null;
        }

        public async Task<List<TVShow>> GetAllTVShows()
        {
            var sqlQuery = GetBasicSelectSql(0);

            return (await GetList(GetConnection(), sqlQuery)).ToList();
        }

        public override async Task<BaseError> Create(TVShow entity)
        {
            var connection = GetConnection();
            try
            {
                await connection.OpenAsync();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + FIELD_NAME +
                                         ", " + FIELD_OVERVIEW +
                                         ", " + FIELD_FIRST_AIR_DATE +
                                         ", " + FIELD_EPISODE_RUN_TIME +
                                         ", " + FIELD_TMDB_ID +
                                         ", " + FIELD_IMDB_ID +
                                         ", " + FIELD_TVDB_ID +
                                         ", " + FIELD_TMDB_POSTER_PATH +
                                         ", " + FIELD_TMDB_BACKDROP_PATH + ") OUTPUT INSERTED.ID VALUES " +
                                         "(@name, @overview, @releaseDate, @runTime, " +
                                         "@tmdbid, @imdbid, @tvdbid, @tmdbPosterPath, @tmdbBackdropPath)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@name", entity.Name),
                    new SqlParameter("@overview", entity.Overview),
                    entity.FirstAirDate.HasValue
                        ? new SqlParameter("@releaseDate", entity.FirstAirDate)
                        : new SqlParameter("@releaseDate", DBNull.Value),
                    new SqlParameter("@runTime", entity.EpisodeRunTime),
                    new SqlParameter("@tmdbid", entity.TmdbId),
                    new SqlParameter("@imdbid", entity.ImdbId),
                    new SqlParameter("@tvdbid", entity.TvdbId),
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

        public override async Task<BaseError> Update(TVShow entity)
        {
            var connection = GetConnection();
            try
            {
                await connection.OpenAsync();

                var insertSql = "UPDATE " + TABLE_NAME + " SET " +
                                         FIELD_NAME  + " =  @name, " +
                                         FIELD_OVERVIEW  + " =  @overview, " +
                                         FIELD_FIRST_AIR_DATE  + " =  @releaseDate, " +
                                         FIELD_EPISODE_RUN_TIME  + " =  @runTime, " +
                                         FIELD_TMDB_ID  + " =  @tmdbid, " +
                                         FIELD_IMDB_ID  + " =  @imdbid, " +
                                         FIELD_TVDB_ID  + " =  @tvdbid, " +
                                         FIELD_TMDB_POSTER_PATH  + " =  @tmdbPosterPath, " +
                                         FIELD_TMDB_BACKDROP_PATH  + " =  @tmdbBackdropPath " +
                                         " WHERE " + ID_FIELD_NAME + " = " + entity.GetId();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@name", entity.Name),
                    new SqlParameter("@overview", entity.Overview),
                    entity.FirstAirDate.HasValue
                        ? new SqlParameter("@releaseDate", entity.FirstAirDate)
                        : new SqlParameter("@releaseDate", DBNull.Value),
                    new SqlParameter("@runTime", entity.EpisodeRunTime),
                    new SqlParameter("@tmdbid", entity.TmdbId),
                    new SqlParameter("@imdbid", entity.ImdbId),
                    new SqlParameter("@tvdbid", entity.TvdbId),
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
    }
}