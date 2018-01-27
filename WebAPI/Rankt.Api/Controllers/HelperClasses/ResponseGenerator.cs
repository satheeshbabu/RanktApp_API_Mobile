using System.Net;
using DataModel.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Rankt.Api.Controllers.HelperClasses
{
    public class ResponseGenerator
    {
        public const string ContentType = "application/json";

        public static ContentResult OkResult(JObject token)
        {
            return new ContentResult
            {
                Content = token.ToString(),
                ContentType = ContentType,
                StatusCode = (int) HttpStatusCode.OK
            };
        }

        public static ContentResult OkResult(BaseEntity entity)
        {
            return new ContentResult
            {
                Content = entity.ToJsonToken().ToString(),
                ContentType = ContentType,
                StatusCode = (int) HttpStatusCode.OK
            };
        }

        #region NotFound

        public static ContentResult NotFoundResult(JObject token)
        {
            return new ContentResult
            {
                Content = token.ToString(),
                ContentType = ContentType,
                StatusCode = (int) HttpStatusCode.NotFound
            };
        }

        public static ContentResult NotFoundResult(string message)
        {
            var token = new JObject { { "message", message } };

            return new ContentResult
            {
                Content = token.ToString(),
                ContentType = ContentType,
                StatusCode = (int) HttpStatusCode.NotFound
            };
        }

        #endregion
    }
}