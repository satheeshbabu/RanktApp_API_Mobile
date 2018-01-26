using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common.Model.Elo;
using Common.Model.Movies;
using DataModel.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Rankt.Api.Repositories.Elo
{
    public class EloListRepository : BaseRepository<EloList>, IEloListRepository
    {
        private const string TABLE_NAME = "dbo.tblEloList";
        private const string ID_FIELD_NAME = "ID";
        private const string FIELD_NAME = "Name";
        private const string FIELD_USER_ID = "UserId";
        private const string FIELD_DATE_CREATED = "Date_Created";
        private const string FIELD_DATE_UPDATED = "Date_Updated";

        private const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_USER_ID + ", " +
            TABLE_NAME + "." + FIELD_DATE_CREATED + ", " +
            TABLE_NAME + "." + FIELD_DATE_UPDATED + " ";

        public EloListRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        private async Task<IEnumerable<EloList>> GetList(SqlConnection connection, string strSql
            , List<SqlParameter> parameters, bool includeListItems)
        {
            var eloLists = new List<EloList>();
            try
            {
                await connection.OpenAsync();
                var command = new SqlCommand(strSql, connection);
                foreach (var sqlParameter in parameters)
                {
                    command.Parameters.Add(sqlParameter);
                }
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var eloList = SerializeFromReader(reader);
                        
                        if (includeListItems)
                        {
                            //TODO Get List Items    
                        }


                        eloLists.Add(eloList);
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

            return eloLists;
        }

        private static EloList SerializeFromReader(IDataRecord reader)
        {
            var eloList = EloList.CreateFromReader(
                long.Parse(reader[ID_FIELD_NAME].ToString()),
                reader[FIELD_NAME].ToString(),
                long.Parse(reader[FIELD_USER_ID].ToString()),
                DateTime.Parse(reader[FIELD_DATE_CREATED].ToString()),
                DateTime.Parse(reader[FIELD_DATE_UPDATED].ToString()));

            return eloList;
        }

        public override async Task<EloList> GetById(long id)
        {
            var sqlParameters = new List<SqlParameter>();

            var sqlQuery = GetBasicSelectSql(1) + " WHERE " +
                           TABLE_NAME + "." + ID_FIELD_NAME + " = @passedInId";

            sqlParameters.Add(new SqlParameter("@passedInId", id));
            var eloLists = (await GetList(GetConnection(), sqlQuery, sqlParameters, true)).ToList();

            return eloLists.Count > 0 ? eloLists[0] : null;
        }

        public Task<EloList> GetByListId(long listId)
        {
            throw new NotImplementedException();
        }

        public Task<List<EloList>> GetAllListsForUser(long userId)
        {
            throw new NotImplementedException();
        }

        public override async Task<BaseError> Create(EloList entity)
        {
            var connection = GetConnection(); ;
            try
            {
                await connection.OpenAsync();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + FIELD_NAME +
                                         ", " + FIELD_USER_ID +
                                         ", " + FIELD_DATE_CREATED +
                                         ", " + FIELD_DATE_UPDATED + ") OUTPUT INSERTED.ID VALUES " +
                                         "(@name, @userId, @createdDate, @updatedDate)";
                var currentDate = DateTime.UtcNow;
                entity.DateCreated = currentDate;
                entity.DateUpdated = currentDate;
                
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@name", entity.Name),
                    new SqlParameter("@userId", entity.UserId),
                    new SqlParameter("@createdDate", entity.DateCreated),
                    new SqlParameter("@updatedDate", entity.DateUpdated)
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

        public override Task<BaseError> Update(EloList entity)
        {
            throw new System.NotImplementedException();
        }
    }
}