using System;
using Common.Model.AuthTokens;
using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace Common.Model.Users
{
    public class CreateUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
    }

    public class User : BaseEntity
    {
        [Category("USER_ENTITY", "User Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }

        public string Username { get; set; }
        public string Password{ get; set; }
        public string EmailAddress{ get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public AuthToken AuthToken { get; set; }

        private User(long id, string username, string password, string emailAddress,
            DateTime createdDate, DateTime updatedDate,bool emailVerified , DateTime? lastLoginDate)
        {
            Id = id;
            Username = username;
            Password = password;
            EmailAddress = emailAddress;
            CreatedDate = createdDate;
            UpdatedDate = updatedDate;
            EmailVerified = emailVerified;
            LastLoginDate = lastLoginDate;
        }

        public static User CreateFromReader(long id, string username, string password, string emailAddress,
            DateTime createdDate, DateTime updatedDate,bool emailVerified, DateTime? lastLoginDate)
        {
            return new User(id,username, password, emailAddress, createdDate, updatedDate, emailVerified, lastLoginDate);
        }

        public static User Instanciate(string username, string password, string emailAddress,
            DateTime createdDate, DateTime updatedDate, bool emailVerified, DateTime? lastLoginDate)
        {
            return new User(0, username, password, emailAddress, createdDate, updatedDate, emailVerified, lastLoginDate);
        }

        public override long GetEntityCategoryId()
        {
            return ENTITY_CATEGORY_ID;
        }

        public override JObject ToJsonToken()
        {
            throw new System.NotImplementedException();
        }
    }
}