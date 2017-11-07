using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Base;
using DataModel.Genres;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using TrakkerApp.Api.Repositories.Genres.MovieGenres;

namespace Trakker.Api.Repositories.Genres.MovieGenres
{
    public class MovieGenreRepository : BaseRepository<MovieGenre>, IMovieGenreRepository
    {
        private const string TABLE_NAME = "dbo.tblGenreMovie";
        private const string ID_FIELD_NAME = "ID";
        private const string FIELD_SOURCE = "Source_Cat";
        private const string FIELD_SOURCE_NAME = "Source_Name";
        private const string FIELD_SOURCE_ID = "Source_Id";

        private const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_SOURCE + ", " +
            TABLE_NAME + "." + FIELD_SOURCE_NAME + ", " +
            TABLE_NAME + "." + FIELD_SOURCE_ID + " ";

        private SqlConnection _connection;

        public MovieGenreRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
            

        }

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        private static MovieGenre SerializeFromReader(IDataRecord reader)
        {
            var movieGenre = MovieGenre.CreateFromReader(
                int.Parse(reader[ID_FIELD_NAME].ToString()),
                int.Parse(reader[FIELD_SOURCE].ToString()),
                reader[FIELD_SOURCE_NAME].ToString(),
                int.Parse(reader[FIELD_SOURCE_ID].ToString()));

            return movieGenre;
        }

        private async Task<IEnumerable<MovieGenre>> GetList(SqlConnection connection, string strSql)
        {
            var movieGenres = new List<MovieGenre>();

            try
            {
                _connection = await GetOpenConnection();
                var command = new SqlCommand(strSql, _connection);

                var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    movieGenres.Add(SerializeFromReader(reader));
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _connection.Close();
            }
            return movieGenres;
        }

        public override async Task<MovieGenre> GetById(long id)
        {
            var sqlQuery = GetBasicSelectSql(0) + " WHERE " +
                           ID_FIELD_NAME + " = " + id;
            var movieGenres = (await GetList(GetConnection(), sqlQuery)).ToList();

            if (movieGenres.Count == 0)
            {
                return null;
            }
            return movieGenres[0];
        }

        public async Task<List<MovieGenre>> GetAllGenresBySource(long source)
        {
            var sqlQuery = GetBasicSelectSql(0) + " WHERE " +
                               FIELD_SOURCE + " = " + source;
            var movieGenres = (await GetList(GetConnection(), sqlQuery)).ToList();

            return movieGenres.Count == 0 ? null : movieGenres;
        }

        public override async Task<BaseError> Create(MovieGenre entity)
        {
            try
            {
                _connection = await GetOpenConnection();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + FIELD_SOURCE +
                                         ", " + FIELD_SOURCE_NAME +
                                         ", " + FIELD_SOURCE_ID + ") OUTPUT INSERTED.ID VALUES " +
                                         "(@source, @sourceName, @sourceId )";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@source", entity.Source),
                    new SqlParameter("@sourceName", entity.SourceName),
                    new SqlParameter("@sourceId", entity.SourceId)
                };

                var command = new SqlCommand(insertSql, _connection);
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
                _connection.Close();
            }
            return entity.GetId() == 0 ? new BaseError(BaseError.Fail) : new BaseError(BaseError.Success);
        }

        public override Task<BaseError> Update(MovieGenre entity)
        {
            throw new System.NotImplementedException();
        }

        
    }
}