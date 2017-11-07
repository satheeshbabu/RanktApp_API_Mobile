using System;
using DataModel.Attributes;
using DataModel.Base;
using DataModel.Movies;
using Newtonsoft.Json.Linq;

namespace DataModel.TVShows
{
    public class TVShow : BaseEntity, IListable
    {
        [Category("TV_SHOW_ENTITY", "TV Show Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }
        
        public string Name { get; set; }
        public string Overview { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public int EpisodeRunTime { get; set; }
        public long TmdbId { get; set; }
        public string ImdbId { get; set; }
        public long TvdbId { get; set; }
        public string TmdbPosterPath { get; set; }
        public string TmdbBackdropPath { get; set; }


        private TVShow(long id, string name, string overview, DateTime? firstAirDate,
            int episodeRunTime, long tmdbId, string imdbId, long tvdbid, string tmdbPoster, 
            string tmdbBackdropPath) : base(id)
        {
            Name = name;
            Overview = overview;
            FirstAirDate = firstAirDate;
            EpisodeRunTime = episodeRunTime;
            TmdbId = tmdbId;
            ImdbId = imdbId;
            TvdbId = tvdbid;
            TmdbPosterPath = tmdbPoster;
            TmdbBackdropPath = tmdbBackdropPath;
        }

        public static TVShow CreateFromReader(long id, string name, string overview, DateTime? firstAirDate, 
            int episodeRunTime,long tmdbId, string imdbId, long tvdbid,  string tmdbPoster, string tmdbBackdrop)
        {
            return new TVShow(id, name, overview, firstAirDate, episodeRunTime, 
                tmdbId, imdbId, tmdbId, tmdbPoster, tmdbBackdrop);
        }

        public static TVShow Instanciate(string name, string overview, DateTime? firstAirDate, 
            int episodeRunTime,long tmdbId, string imdbId,  string tmdbPoster, string tmdbBackdrop)
        {
            return new TVShow(0, name, overview, firstAirDate, episodeRunTime, 
                tmdbId, imdbId, 0, tmdbPoster, tmdbBackdrop);
        }

        public override JObject ToJsonToken()
        {
            var token = new JObject
            {
                {"name", Name},
                { "id", Id},
                { "overview", Overview},
                { "first_air_date", FirstAirDate},
                { "episode_run_time", EpisodeRunTime},
                {"tmdb_poster_path", TmdbPosterPath },
                {"imdb_id", ImdbId }
            };
            return token;
        }
        public override long GetEntityCategoryId()
        {
            return ENTITY_CATEGORY_ID;
        }
        public JObject GetListableJsonToken()
        {
            var token = ToJsonToken();
            token.Add("type", "TV SHOW");

            return token;
        }
    }
}