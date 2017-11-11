using System;
using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace Common.Model.AuthTokens
{
    public class AuthToken : BaseEntity
    {
        [Category("AUTH_TOKEN_ENTITY", "Auth Token Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }

        public string Token { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUsedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        private AuthToken(long id, string token, long userId, DateTime createdDate, DateTime lastUsedDate,
            DateTime expiryDate)
        {
            Id = id;
            Token = token;
            UserId = userId;
            CreatedDate = createdDate;
            LastUsedDate = lastUsedDate;
            ExpiryDate = expiryDate;
        }

        public static AuthToken CreateFromReader(long id, string token, long userId, DateTime createdDate, DateTime lastUsedDate,
            DateTime expiryDate)
        {
            return new AuthToken(id, token, userId,createdDate, lastUsedDate, expiryDate);
        }

        public static AuthToken Instanciate( string token, long userId, DateTime createdDate, DateTime lastUsedDate,
            DateTime expiryDate)
        {
            return new AuthToken(0, token, userId, createdDate, lastUsedDate, expiryDate);
        }

        public override long GetEntityCategoryId()
        {
            return ENTITY_CATEGORY_ID;
        }

        public override JObject ToJsonToken()
        {
            throw new NotImplementedException();
        }
    }
}