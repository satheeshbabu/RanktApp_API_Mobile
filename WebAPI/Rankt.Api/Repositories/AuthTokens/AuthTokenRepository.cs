using System.Threading.Tasks;
using Common.Model.AuthTokens;
using DataModel.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Trakker.Api.Repositories;

namespace Rankt.Api.Repositories.AuthTokens
{
    public class AuthTokenRepository: BaseRepository<AuthToken>, IAuthTokenRepository
    {
        private const string TABLE_NAME = "tblAuthToken";
        private const string ID_FIELD_NAME = "ID";
        private const string FIELD_TOKEN = "Token";
        private const string FIELD_USER_ID = "User_ID";
        private const string FIELD_CREATED_DATE = "Date_Created";
        private const string FIELD_LAST_USED_DATE = "Date_Last_Used";
        private const string FIELD_EXPIRY_DATE = "Date_Expire";

        private const string ALL_FIELDS =
            TABLE_NAME + "." + ID_FIELD_NAME + ", " +
            TABLE_NAME + "." + FIELD_TOKEN + ", " +
            TABLE_NAME + "." + FIELD_USER_ID + ", " +
            TABLE_NAME + "." + FIELD_CREATED_DATE + ", " +
            TABLE_NAME + "." + FIELD_LAST_USED_DATE + ", " +
            TABLE_NAME + "." + FIELD_EXPIRY_DATE + " ";


        public AuthTokenRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {

        }

        private static string GetBasicSelectSql(int limitResults)
        {
            return "SELECT " + (limitResults == 0 ? "" : " TOP " + limitResults + " ") + ALL_FIELDS + " FROM " + TABLE_NAME;
        }

        public override Task<AuthToken> GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public Task<AuthToken> GetByUserId(long userId)
        {
            throw new System.NotImplementedException();
        }

        public override Task<BaseError> Create(AuthToken entity)
        {
            throw new System.NotImplementedException();
        }

        public override Task<BaseError> Update(AuthToken entity)
        {
            throw new System.NotImplementedException();
        }
    }
}