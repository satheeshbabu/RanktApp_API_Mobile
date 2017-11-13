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
using TrakkerApp.Api.Repositories.Genres.TVShowGenres;

namespace Rankt.Api.Repositories.Genres.TVShowGenres
{
    public class TVShowGenreRepository : BaseRepository<TVShowGenre>, ITVShowGenreRepository
    {
        private const string TABLE_NAME = "dbo.tblGenreTvShow";
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

        public TVShowGenreRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }
        
        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        public static async Task<IEnumerable<TVShowGenre>> GetList(SqlConnection connection, string strSql)
        {
            var tvShowGenres = new List<TVShowGenre>();
            try
            {
                await connection.OpenAsync();
                var command = new SqlCommand(strSql, connection);
                command.Prepare();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tvShowGenres.Add(SerializeFromReader(reader));
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
            return tvShowGenres;
        }

        private static TVShowGenre SerializeFromReader(IDataRecord reader)
        {
            return TVShowGenre.CreateFromReader(
                int.Parse(reader[ID_FIELD_NAME].ToString()),
                int.Parse(reader[FIELD_SOURCE].ToString()),
                reader[FIELD_SOURCE_NAME].ToString(),
                int.Parse(reader[FIELD_SOURCE_ID].ToString()));
        }
        
        

        public override Task<TVShowGenre> GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<TVShowGenre>> GetAllGenresBySource(long source)
        {
            var sqlQuery = GetBasicSelectSql(0) + " WHERE " +
                               FIELD_SOURCE + " = " + source;

            return (await GetList(GetConnection(), sqlQuery)).ToList();
        }

        public override async Task<BaseError> Create(TVShowGenre entity)
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

        public override Task<BaseError> Update(TVShowGenre entity)
        {
            throw new System.NotImplementedException();
        }
    }
}