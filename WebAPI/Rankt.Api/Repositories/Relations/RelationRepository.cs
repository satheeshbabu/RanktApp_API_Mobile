using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using DataModel.Base;
using DataModel.Genres;
using DataModel.Movies;
using DataModel.Overall;
using DataModel.TVShows;
using Microsoft.Extensions.Caching.Memory;
using Trakker.Api.Repositories;

namespace TrakkerApp.Api.Repositories.Relations
{
    public class RelationRepository : BaseRepository<Relation>, IRelationRepository
    {
        public const string TABLE_NAME = "dbo.tblRelation";
        public const string ID_FIELD_NAME = "ID";
        public const string TABLE_COLUMN_CAT_FROM = "Cat_From";
        public const string TABLE_COLUMN_ENTITY_FROM = "Entity_From";
        public const string TABLE_COLUMN_CAT_TO = "Cat_To";
        public const string TABLE_COLUMN_ENTITY_TO = "Entity_To";
        public const string TABLE_COLUMN_REL_STATUS = "Rel_Status";
        public const string TABLE_COLUMN_CREATED_DATE = "Created_Date";

        public const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_CAT_FROM + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_ENTITY_FROM + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_CAT_TO + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_ENTITY_TO + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_REL_STATUS + ", " +
            TABLE_NAME + "." + TABLE_COLUMN_CREATED_DATE + " ";

        private SqlConnection _connection;

        public RelationRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {

        }

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }
        
        private static async Task<IEnumerable<Relation>> GetList(SqlConnection connection, string strSql)
        {
            var relations = new List<Relation>();

//            Console.WriteLine("Relation query");
//            Console.WriteLine(strSql);
            try
            {
                await connection.OpenAsync();
                var command = new SqlCommand(strSql, connection);
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        relations.Add(SerializeFromReader(reader));
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
//            Console.WriteLine("Relations size = " + relations.Count);
            return relations;
        }
        
        private static Relation SerializeFromReader(IDataRecord reader)
        {
            DateTime.TryParse(reader[TABLE_COLUMN_CREATED_DATE].ToString(), out DateTime d2);

            var relation = Relation.CreateFromReader(
                long.Parse(reader[ID_FIELD_NAME].ToString()),
                long.Parse(reader[TABLE_COLUMN_CAT_FROM].ToString()),
                long.Parse(reader[TABLE_COLUMN_ENTITY_FROM].ToString()),
                long.Parse(reader[TABLE_COLUMN_CAT_TO].ToString()),
                long.Parse(reader[TABLE_COLUMN_ENTITY_TO].ToString()),
                long.Parse(reader[TABLE_COLUMN_REL_STATUS].ToString()),
                d2
                );

            return relation;
        }

        public override Task<Relation> GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<Relation>> GetRelationsByParent(long parentCatId, long parentEntityId)
        {
            var sqlQuery = GetBasicSelectSql(0) + " WHERE " +
                           TABLE_NAME + "." + TABLE_COLUMN_CAT_FROM + " = " + parentCatId + " AND " +
                           TABLE_NAME + "." + TABLE_COLUMN_ENTITY_FROM + " = " + parentEntityId;
            
            return (await GetList(GetConnection(), sqlQuery)).ToList();
        }

        public async Task CreateMediaListToMovieRelationship(MediaList mediaList, Movie movie)
        {
            var relation = Relation.Instanciate(MediaList.ENTITY_CATEGORY_ID, mediaList.GetId(),
                Movie.ENTITY_CATEGORY_ID, movie.GetId(), Relation.RELATION_STATUS_ACTIVE_ID, DateTime.Now);
            await Save(relation);
        }

        public async Task CreateMediaListToTVShowRelationship(MediaList mediaList, TVShow tvShow)
        {
            var relation = Relation.Instanciate(MediaList.ENTITY_CATEGORY_ID, mediaList.GetId(),
                TVShow.ENTITY_CATEGORY_ID, tvShow.GetId(), Relation.RELATION_STATUS_ACTIVE_ID, DateTime.Now);
            await Save(relation);
        }

        public async Task CreateMovieCollectionToMovieRelationship(MovieCollection movieCollection, Movie movie)
        {
            var relation = Relation.Instanciate(MovieCollection.ENTITY_CATEGORY_ID, movieCollection.GetId(),
                Movie.ENTITY_CATEGORY_ID, movie.GetId(), Relation.RELATION_STATUS_ACTIVE_ID, DateTime.Now);
            await Save(relation);
        }

        public async Task CreateMovieToMovieGenreRelationship(Movie movie, MovieGenre movieGenre)
        {
            var relation = Relation.Instanciate(Movie.ENTITY_CATEGORY_ID, movie.GetId(),
                MovieGenre.ENTITY_CATEGORY_ID, movieGenre.GetId(), Relation.RELATION_STATUS_ACTIVE_ID,
                DateTime.Now);
            await Save(relation);
        }

        public async Task CreateTVShowToTVShowGenreRelationship(TVShow tvShow, TVShowGenre tvShowGenre)
        {
            var relation = Relation.Instanciate(TVShow.ENTITY_CATEGORY_ID, tvShow.GetId(),
                TVShowGenre.ENTITY_CATEGORY_ID, tvShowGenre.GetId(), Relation.RELATION_STATUS_ACTIVE_ID,
                DateTime.Now);
            await Save(relation);
        }

        public override async Task<BaseError> Create(Relation entity)
        {
            try
            {
                _connection = await GetOpenConnection();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + TABLE_COLUMN_CAT_FROM +
                                         ", " + TABLE_COLUMN_ENTITY_FROM +
                                         ", " + TABLE_COLUMN_CAT_TO +
                                         ", " + TABLE_COLUMN_ENTITY_TO +
                                         ", " + TABLE_COLUMN_REL_STATUS +
                                         ", " + TABLE_COLUMN_CREATED_DATE + ") OUTPUT INSERTED.ID VALUES " +
                                         "(@catFrom, @entFrom, @catTo, @entTo, @relStatus, @createdAt)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@catFrom", entity.CategoryFrom),
                    new SqlParameter("@entFrom", entity.EntityFrom),
                    new SqlParameter("@catTo", entity.CategoryTo),
                    new SqlParameter("@entTo", entity.EntityTo),
                    new SqlParameter("@relStatus", entity.RelationStatus),
                    new SqlParameter("@createdAt", entity.CreatedDate)
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

        public override Task<BaseError> Update(Relation entity)
        {
            throw new System.NotImplementedException();
        }
    }
}