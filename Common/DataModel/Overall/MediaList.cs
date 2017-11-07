using System.Collections.Generic;
using DataModel.Attributes;
using DataModel.Base;
using Newtonsoft.Json.Linq;

namespace DataModel.Overall
{
    public class MediaList : BaseEntity
    {
        [Category("LIST_ENTITY", "List Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }
        [Category("MEDIA_LIST_TYPE_LIST", "Types of Media List")]
        public static long MEDIA_LIST_TYPE_LIST { get; set; }
        [Category("MEDIA_LIST_ONLY_MOVIES", "List Only Movies", "MEDIA_LIST_TYPE_LIST")]
        public static long MEDIA_LIST_ONLY_MOVIES { get; set; }
        [Category("MEDIA_LIST_ONLY_TV", "List Only TV", "MEDIA_LIST_TYPE_LIST")]
        public static long MEDIA_LIST_ONLY_TV { get; set; }

        public long ListType { get; set; }
        public long Source { get; set; }
        public string SourceId { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }

        public List<IListable> MediaListables;

        private MediaList(long id, long listType , long source, string sourceId, string name,
            string overview, string posterPath, string backdropPath) : base(id)
        {
            Id = id;
            ListType = listType;
            Source = source;
            SourceId = sourceId;
            Name = name;
            Overview = overview;
            PosterPath = posterPath;
            BackdropPath = backdropPath;
            
            MediaListables = new List<IListable>();
        }

        public static MediaList CreateFromReader(long id, long listType, long source, string sourceId, string name,
            string overview, string posterPath, string backdropPath)
        {
            return new MediaList(id, listType, source, sourceId, name, overview, posterPath, backdropPath);
        }

        public static MediaList InstanciateImdbMovieList(string sourceId, string name, string overview,
            string posterPath, string backdropPath)
        {
            return new MediaList(0, MEDIA_LIST_ONLY_MOVIES, Category.SOURCE_IMDB, sourceId,
                name, overview, posterPath, backdropPath);
        }
        
        public static MediaList InstanciateImdbTVShowList(string sourceId, string name, string overview,
            string posterPath, string backdropPath)
        {
            return new MediaList(0, MEDIA_LIST_ONLY_TV, Category.SOURCE_IMDB, sourceId,
                name, overview, posterPath, backdropPath);
        }

        public void AddListableItem(IListable listableItem)
        {
            MediaListables.Add(listableItem);
        }

        public override long GetEntityCategoryId()
        {
            return ENTITY_CATEGORY_ID;
        }

        public override JObject ToJsonToken()
        {
            var token = new JObject
            {
                {"id", Id},
                {"list_type", ListType},
                {"source", Source},
                {"source_id", SourceId},
                {"name", Name},
                { "overview", Overview}
            };
            
            var listablesArray = new JArray();
            foreach (var mediaListable in MediaListables)
            {
                listablesArray.Add(mediaListable.GetListableJsonToken());
            }
            token.Add("listable", listablesArray);
            
            return token;
        }
    }
}