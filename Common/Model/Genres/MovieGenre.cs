using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace DataModel.Genres
{
    public class MovieGenre : Genre
    {
        [Category("GENRE_MOVIE_ENTITY", "Movie Genre Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }
        
        private MovieGenre(long id, long source, string sourceName, long sourceId) : 
            base(id, source, sourceName, sourceId)
        {
           //Movie Specific tasks, at some point
        }

        public static MovieGenre CreateFromReader(long id, long source, string sourceName, long sourceId)
        {
            return new MovieGenre(id, source, sourceName, sourceId);
        }

        public static MovieGenre InstanciateTmdbGenreMovie(string genreName, long genreId)
        {
            return new MovieGenre(0, Category.SOURCE_TMDB, genreName, genreId);
        }

        public override long GetEntityCategoryId()
        {
            throw new System.NotImplementedException();
        }

        public override JObject ToJsonToken()
        {
            return new JObject
            {
                {"id", Id},
                {"source", Source},
                {"source_id", SourceId},
                {"name", SourceName}
            };
        }
    }
}