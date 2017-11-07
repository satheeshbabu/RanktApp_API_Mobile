using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataModel.Base;
using DataModel.Movies;
using Microsoft.Extensions.Configuration;
using TrakkerApp.Api.Repositories;

namespace Trakker.Api.Repositories.Movies
{
    public class MovieCollectionRepository : BaseRepository<MovieCollection>, IMovieCollectionRepository
    {
        public const string TABLE_NAME = "dbo.tblMovieCollection";
        public const string ID_FIELD_NAME = "ID";
        public const string TABLE_COLUMN_SOURCE = "Source";
        public const string TABLE_COLUMN_SOURCE_ID = "Source_Id";
        public const string TABLE_COLUMN_NAME = "Name";
        public const string TABLE_COLUMN_OVERVIEW = "Overview";
        public const string TABLE_COLUMN_POSTER_PATH = "Poster";
        public const string TABLE_COLUMN_BACKDROP_PATH = "Backdrop";

        public const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_SOURCE + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_SOURCE_ID + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_NAME + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_OVERVIEW + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_POSTER_PATH + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_BACKDROP_PATH + " ";

        private SqlConnection _connection;

        public MovieCollectionRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public static string GetBasicSelectSql()
        {
            return "SELECT " + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        private static string GetSingleBasicSelectSql()
        {
            return "SELECT TOP 1 " + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        public override Task<MovieCollection> GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<MovieCollection> GetBySourceAndMovieCollectionId(long source, long id)
        {
            MovieCollection movieCollection = null;
            try
            {
                _connection = await GetOpenConnection();

                var sqlQuery = GetSingleBasicSelectSql() + " WHERE " +
                               TABLE_NAME + "." + TABLE_COLUMN_SOURCE + " = @passedSource AND " +
                               TABLE_NAME + "." + TABLE_COLUMN_SOURCE_ID + " = @passedId";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@passedSource", source),
                    new SqlParameter("@passedId", id)
                };

                var command = new SqlCommand(sqlQuery, _connection);

                foreach (var sqlParameter in parameters)
                {
                    command.Parameters.Add(sqlParameter);
                }

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    movieCollection = MovieCollection.CreateFromReader(int.Parse(reader[ID_FIELD_NAME].ToString()),
                        Int32.Parse(reader[TABLE_COLUMN_SOURCE].ToString()),
                        Int32.Parse(reader[TABLE_COLUMN_SOURCE_ID].ToString()),
                        reader[TABLE_COLUMN_NAME].ToString(),
                        reader[TABLE_COLUMN_OVERVIEW].ToString(),
                        reader[TABLE_COLUMN_POSTER_PATH].ToString(),
                        reader[TABLE_COLUMN_BACKDROP_PATH].ToString());
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
            return movieCollection;
        }

        public override async Task<BaseError> Create(MovieCollection entity)
        {
            try
            {
                _connection = await GetOpenConnection();

                string insertSql = "INSERT INTO " + TABLE_NAME +
                                   " (" + TABLE_COLUMN_SOURCE +
                                   ", " + TABLE_COLUMN_SOURCE_ID +
                                   ", " + TABLE_COLUMN_NAME +
                                   ", " + TABLE_COLUMN_OVERVIEW +
                                   ", " + TABLE_COLUMN_POSTER_PATH +
                                   ", " + TABLE_COLUMN_BACKDROP_PATH + ") OUTPUT INSERTED.ID VALUES " +
                                   "(@source, @sourceId ,@name, @overview,  @poster, @backdrop)";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@source", entity.Source),
                    new SqlParameter("@sourceId", entity.SourceId),
                    new SqlParameter("@name", entity.Name),
                    new SqlParameter("@overview", entity.Overview),
                    new SqlParameter("@poster", entity.PosterPath),
                    new SqlParameter("@backdrop", entity.BackdropPath)
                };


                SqlCommand command = new SqlCommand(insertSql, _connection);

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

        public override Task<BaseError> Update(MovieCollection entity)
        {
            throw new System.NotImplementedException();
        }
    }
}