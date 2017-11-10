using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace DataModel.Genres
{
    public class TVShowGenre : Genre
    {
        [Category("GENRE_TV_SHOW_ENTITY", "TV Show Genre Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }
        
        private TVShowGenre(long id, long source, string sourceName, long sourceId) : 
            base(id, source, sourceName, sourceId)
        {
            //Movie Specific tasks, at some point
        }

        public static TVShowGenre CreateFromReader(long id, long source, string sourceName, long sourceId)
        {
            return new TVShowGenre(id, source, sourceName, sourceId);
        }

        public static TVShowGenre InstanciateTmdbGenreTVShow(string genreName, long genreId)
        {
            return new TVShowGenre(0, Category.SOURCE_TMDB, genreName, genreId);
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