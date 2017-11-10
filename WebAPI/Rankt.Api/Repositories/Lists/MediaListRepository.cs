using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common.Model.Movies;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using DataModel.Movies;
using DataModel.Overall;
using DataModel.TVShows;
using Trakker.Api.Repositories;
using Trakker.Api.Repositories.Movies;
using TrakkerApp.Api.Repositories.Relations;
using TrakkerApp.Api.Repositories.TVShows;

namespace TrakkerApp.Api.Repositories.Lists
{
    public class MediaListRepository : BaseRepository<MediaList> , IMediaListRepository
    {
        public const string TABLE_NAME = "dbo.tblMediaList";
        public const string ID_FIELD_NAME = "ID";
        public const string TABLE_COLUMN_LIST_TYPE_CAT = "List_Type_Cat";
        public const string TABLE_COLUMN_SOURCE = "Source";
        public const string TABLE_COLUMN_SOURCE_ID = "Source_Id";
        public const string TABLE_COLUMN_NAME = "Name";
        public const string TABLE_COLUMN_OVERVIEW = "Overview";
        public const string TABLE_COLUMN_POSTER_PATH = "Poster";
        public const string TABLE_COLUMN_BACKDROP_PATH = "Backdrop";

        public const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_LIST_TYPE_CAT + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_SOURCE + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_SOURCE_ID + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_NAME + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_OVERVIEW + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_POSTER_PATH + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_BACKDROP_PATH + " ";

        private SqlConnection _connection;

        public MediaListRepository(IConfiguration configuration) : base(configuration)
        {

        }

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        private async Task<IEnumerable<MediaList>> GetList(SqlConnection connection, string strSql, bool includeMediaElements)
        {
            var mediaLists = new List<MediaList>();
            try
            {
                await connection.OpenAsync();
                var command = new SqlCommand(strSql, connection);
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var mediaList = SerializeFromReader(reader);

                        if (includeMediaElements)
                        {
                            await GetListableItems(mediaList);
                        }

                        mediaLists.Add(mediaList);
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
            return mediaLists;
        }
        
        private static MediaList SerializeFromReader(IDataRecord reader)
        {
            var mediaList = MediaList.CreateFromReader(Int32.Parse(reader[ID_FIELD_NAME].ToString()),
                Int32.Parse(reader[TABLE_COLUMN_LIST_TYPE_CAT].ToString()),
                Int32.Parse(reader[TABLE_COLUMN_SOURCE].ToString()),
                reader[TABLE_COLUMN_SOURCE_ID].ToString(),
                reader[TABLE_COLUMN_NAME].ToString(),
                reader[TABLE_COLUMN_OVERVIEW].ToString(),
                reader[TABLE_COLUMN_POSTER_PATH].ToString(),
                reader[TABLE_COLUMN_BACKDROP_PATH].ToString());

            return mediaList;
        }

        public override async Task<MediaList> GetById(long id)
        {
            var sqlQuery = GetBasicSelectSql(1) + " WHERE " +
                           TABLE_NAME + "." + ID_FIELD_NAME + " = '" + id + "'";
            //TODO figure out best passing in boolean
            var mediaList = (await GetList(GetConnection(), sqlQuery, true)).ToList();

            if (mediaList.Count == 0)
            {
                return null;
            }

            return mediaList[0];
        }

        public async Task GetListableItems(MediaList mediaList)
        {
            var relationRepository = new RelationRepository(Configuration, MemoryCache);

            var relList = await relationRepository.GetRelationsByParent(
                mediaList.GetEntityCategoryId(), mediaList.GetId());
            //TODO FIX!!!!
            var movieRepository = new MovieRepository(Configuration,MemoryCache);
            var tvShowRepository = new TVShowRepository(Configuration);

            foreach (var relation in relList)
            {
                if (relation.CategoryTo == Movie.ENTITY_CATEGORY_ID)
                {
                    mediaList.AddListableItem(await movieRepository.GetById(relation.EntityTo));
                }
                else if (relation.CategoryTo == TVShow.ENTITY_CATEGORY_ID)
                {
                    mediaList.AddListableItem(await tvShowRepository.GetById(relation.EntityTo));
                }
            }
        }

        public async Task<MediaList> GetBySourceAndCollectionId(long source, string id, bool includeMediaElements)
        {
            var sqlQuery = GetBasicSelectSql(1) + " WHERE " +
                               TABLE_NAME + "." + TABLE_COLUMN_SOURCE + " = " + source + " AND " +
                               TABLE_NAME + "." + TABLE_COLUMN_SOURCE_ID + " = '" + id + "'";

            var mediaList = (await GetList(GetConnection(), sqlQuery, includeMediaElements)).ToList();

            return mediaList.Count > 0 ? mediaList[0] : null;
        }

        public async Task<List<MediaList>> GetAllLists(bool getMediaElements)
        {
            var sqlQuery = GetBasicSelectSql(0);

            return (await GetList(GetConnection(), sqlQuery, getMediaElements)).ToList();
        }

        public override async Task<BaseError> Create(MediaList entity)
        {
            try
            {
                _connection = await GetOpenConnection();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + TABLE_COLUMN_LIST_TYPE_CAT +
                                         ", " + TABLE_COLUMN_SOURCE +
                                         ", " + TABLE_COLUMN_SOURCE_ID +
                                         ", " + TABLE_COLUMN_NAME +
                                         ", " + TABLE_COLUMN_OVERVIEW +
                                         ", " + TABLE_COLUMN_POSTER_PATH +
                                         ", " + TABLE_COLUMN_BACKDROP_PATH + ") OUTPUT INSERTED.ID VALUES " +
                                         "(@listType, @source, @sourceId ,@name, @overview, @poster, @backdrop)";

                var command = new SqlCommand(insertSql, _connection);
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@listType", entity.ListType),
                    new SqlParameter("@source", entity.Source),
                    new SqlParameter("@sourceId", entity.SourceId),
                    new SqlParameter("@name", entity.Name),
                    string.IsNullOrEmpty(entity.Overview)?
                        new SqlParameter("@overview", DBNull.Value):
                        new SqlParameter("@overview", entity.Overview),
                    string.IsNullOrEmpty(entity.PosterPath)?
                        new SqlParameter("@poster", DBNull.Value):
                        new SqlParameter("@poster", entity.PosterPath),
                    string.IsNullOrEmpty(entity.BackdropPath)?
                        new SqlParameter("@backdrop", DBNull.Value):
                        new SqlParameter("@backdrop", entity.BackdropPath)
                };

                foreach (var sqlParameter in parameters)
                {
                    command.Parameters.Add(sqlParameter);
                }
                entity.SetId( Convert.ToInt32(await command.ExecuteScalarAsync()));
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

        public override Task<BaseError> Update(MediaList entity)
        {
            throw new System.NotImplementedException();
        }
    }
}