using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common.Model.Movies;
using Common.Model.Users;
using DataModel.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Trakker.Api.Repositories;

namespace Rankt.Api.Repositories.Users
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private const string TABLE_NAME = "tblAuthToken";
        private const string ID_FIELD_NAME = "ID";
        private const string FIELD_USERNAME = "Username";
        private const string FIELD_PASSWORD = "Password";
        private const string FIELD_EMAIL_ADDRESS = "Email_Address";
        private const string FIELD_CREATED_DATE = "Date_Created";
        private const string FIELD_UPDATED_DATE = "Date_Updated";
        private const string FIELD_EMAIL_VERIFIED = "Email_Verified";
        private const string FIELD_LAST_LOGIN_DATE = "Last_Login";

        private const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_USERNAME + ", " +
            TABLE_NAME + "." + FIELD_PASSWORD + ", " +
            TABLE_NAME + "." + FIELD_EMAIL_ADDRESS + ", " +
            TABLE_NAME + "." + FIELD_CREATED_DATE + ", " +
            TABLE_NAME + "." + FIELD_UPDATED_DATE + ", " +
            TABLE_NAME + "." + FIELD_EMAIL_VERIFIED + ", " +
            TABLE_NAME + "." + FIELD_LAST_LOGIN_DATE + " ";

        public UserRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        private async Task<IEnumerable<User>> GetList(SqlConnection connection, string strSql
            , List<SqlParameter> parameters)
        {
            var users = new List<User>();
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
                        var user = SerializeFromReader(reader);
                        //TODO Get Auth Token
                        //await GetMovieGenres(movie);

                        users.Add(user);
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

            return users;
        }

        private static User SerializeFromReader(IDataRecord reader)
        {
            var movie = User.CreateFromReader(
                long.Parse(reader[ID_FIELD_NAME].ToString()),
                reader[FIELD_USERNAME].ToString(),
                reader[FIELD_PASSWORD].ToString(),
                reader[FIELD_EMAIL_ADDRESS].ToString(),
                DateTime.Parse(reader[FIELD_CREATED_DATE].ToString()),
                DateTime.Parse(reader[FIELD_UPDATED_DATE].ToString()),
                (bool)reader[FIELD_EMAIL_VERIFIED],
                DateTime.Parse(reader[FIELD_LAST_LOGIN_DATE].ToString()));

            return movie;
        }

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        public override Task<User> GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> GetUserByNameAndPassword(string username, string password)
        {
            var sqlParameters = new List<SqlParameter>();

            var sqlQuery = GetBasicSelectSql(1) + " WHERE " +
                           TABLE_NAME + "." + FIELD_USERNAME + " = @passedInUser" + " AND " +
                           TABLE_NAME + "." + FIELD_PASSWORD + " = @passedInPassword";

            sqlParameters.Add(new SqlParameter("@passedInUser", username));
            sqlParameters.Add(new SqlParameter("@passedInPassword", password));

            var users = (await GetList(GetConnection(), sqlQuery, sqlParameters)).ToList();

            //TODO Get token here, if doesn't exsist, create it

            return users.Count > 0 ? users[0] : null;

        }

        public override async Task<BaseError> Create(User entity)
        {
            var connection = GetConnection(); ;
            try
            {
                await connection.OpenAsync();

                const string insertSql = "INSERT INTO " + TABLE_NAME +
                                         " (" + FIELD_USERNAME + 
                                         ", " + FIELD_PASSWORD + 
                                         ", " + FIELD_EMAIL_ADDRESS + 
                                         ", " + FIELD_CREATED_DATE + 
                                         ", " + FIELD_UPDATED_DATE + 
                                         ", " + FIELD_EMAIL_VERIFIED +
                                         ", " + FIELD_LAST_LOGIN_DATE + ") OUTPUT INSERTED.ID VALUES " +
                                         "( @username, @password, @email, " +
                                         "@created, @updated, @emailVerified, @lastLogin)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@name", entity.Username),
                    new SqlParameter("@password", entity.Password),
                    new SqlParameter("@email", entity.EmailAddress),
                    new SqlParameter("@created", entity.CreatedDate),
                    new SqlParameter("@updated", entity.UpdatedDate),
                    new SqlParameter("@emailVerified", entity.EmailVerified),
                    new SqlParameter("@lastLogin", DBNull.Value),
                    
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

        public override Task<BaseError> Update(User entity)
        {
            throw new System.NotImplementedException();
        }
    }
}