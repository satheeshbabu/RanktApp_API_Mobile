using System.Threading.Tasks;
using Common.Model.Users;
using DataModel.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Trakker.Api.Repositories.Users
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

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        public override Task<User> GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetUserByNameAndPassword(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public override Task<BaseError> Create(User entity)
        {
            throw new System.NotImplementedException();
        }

        public override Task<BaseError> Update(User entity)
        {
            throw new System.NotImplementedException();
        }
    }
}