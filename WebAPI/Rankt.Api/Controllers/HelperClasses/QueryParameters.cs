using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TrakkerApp.Api.Controllers.HelperClasses
{
    public class QueryPagenationParameters
    {
        [BindRequired]
        public int Page { get; set; }
        [BindRequired]
        public int ResultsPerPage { get; set; }
    }

    public class QueryLanguageParameters
    {
        [BindRequired]
        public string Lang { get; set; }
    }
}