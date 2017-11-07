using DataModel.Attributes;
using Newtonsoft.Json.Linq;

namespace DataModel.Base
{
    public class Category : BaseEntity
    {
        [Category("ENTITY", "Category Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }

        [Category("SOURCE_LIST", "List of Sources")]
        public static long SOURCE_LIST { get; set; }

        [Category("SOURCE_IMDB", "Source is TMDB", "SOURCE_LIST")]
        public static long SOURCE_TMDB { get; set; }
        
        [Category("SOURCE_TMDB", "Source is IMDB", "SOURCE_LIST")]
        public static long SOURCE_IMDB { get; set; }
        
        public string Name { get; set; }
        public string SimpleName { get; set; }
        public string Status { get; set; }

        //TODO make private
        private Category(long id, string name, string simpleName, string status)
        {
            Id = id;
            Name = name;
            SimpleName = simpleName;
            Status = status;
        }

        public static Category InstanciateFromReader(long id, string name, string simpleName,
            string status)
        {
            return new Category(id, name, simpleName, status);
        }

        public static Category Instanciate(string name, string simpleName,
            string status)
        {
            return new Category(0, name, simpleName, status);
        }

        public override long GetEntityCategoryId()
        {
            throw new System.NotImplementedException();
        }

        public override JObject ToJsonToken()
        {
            throw new System.NotImplementedException();
        }
    }
}