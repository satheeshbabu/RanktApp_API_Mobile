using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace DataModel.Movies
{
    public class MovieCollection : BaseEntity
    {
        [Category("MOVIE_COLLECTION_ENTITY", "Movie Collection Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }
        
        public long Source { get; set; }
        public long SourceId { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
        
        private MovieCollection(long id, long source, long sourceId, string name, 
            string overview, string posterPath, string backdropPath) : base(id)
        {
            Source = source;
            SourceId = sourceId;
            Name = name;
            Overview = overview;
            PosterPath = posterPath;
            BackdropPath = backdropPath;
        }

        public static MovieCollection CreateFromReader(long id, long source, long sourceId, string name, 
            string overview, string posterPath, string backdropPath)
        {
            return new MovieCollection(id, source, sourceId, name, overview, posterPath, backdropPath);
        }

        public static MovieCollection InstanciateTmdbCollection(long sourceId, string name, string overview,
            string posterPath, string backdropPath)
        {
            return new MovieCollection(0, Category.SOURCE_TMDB, sourceId, 
                name, overview, posterPath, backdropPath);
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