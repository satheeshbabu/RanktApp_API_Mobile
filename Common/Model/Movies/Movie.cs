using System;
using System.Collections.Generic;
using DataModel.Attributes;
using DataModel.Base;
using DataModel.Genres;
using Newtonsoft.Json.Linq;

namespace Common.Model.Movies
{
    public class Movie : BaseEntity, IListable
    {
        [Category("MOVIE_ENTITY", "Movie Entity")]
        public static long ENTITY_CATEGORY_ID { get; set; }
        
        public string Name { get; set; }
        public string Overview { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int RunTime { get; set; }
        public long TmdbId { get; set; }
        public string ImdbId { get; set; }
        public string TmdbPosterPath { get; set; }
        public string TmdbBackdropPath { get; set; }
        public DateTime DateUpdated { get; set; }

        public List<MovieGenre> Genres = new List<MovieGenre>();


        private Movie(long id, string name, string overview, DateTime? releaseDate,
            int runTime, long tmdbId, string imdbId, string tmdbPoster, 
            string tmdbBackdropPath, DateTime dateUpdated) : base(id)
        {
            Name = name;
            Overview = overview;
            ReleaseDate = releaseDate;
            RunTime = runTime;
            TmdbId = tmdbId;
            ImdbId = imdbId;
            TmdbPosterPath = tmdbPoster;
            TmdbBackdropPath = tmdbBackdropPath;
            DateUpdated = dateUpdated;
        }

        public static Movie CreateFromReader(long id, string name, string overview, DateTime? releaseDate, int runTime,
            long tmdbId, string imdbId, string tmdbPoster, string tmdbBackdrop, DateTime dateUpdated)
        {
            return new Movie(id, name, overview, releaseDate, runTime, 
                tmdbId, imdbId, tmdbPoster, tmdbBackdrop, dateUpdated);
        }

        public static Movie Instanciate(string name, string overview, DateTime? releaseDate, int runTime,
            long tmdbId, string imdbId, string tmdbPoster, string tmdbBackdrop, DateTime dateUpdated)
        {
            return new Movie(0, name, overview, releaseDate, runTime, 
                tmdbId, imdbId, tmdbPoster, tmdbBackdrop, dateUpdated);
        }

        public override JObject ToJsonToken()
        {
            var token = new JObject
            {
                {"name", Name},
                { "id", Id},
                { "overview", Overview},
                { "release_date", ReleaseDate?.Year},
                {"tmdb_poster_path", TmdbPosterPath },
                {"imdb_id", ImdbId },
                {"date_upated", DateUpdated }
            };

            var listablesArray = new JArray();
            foreach (var genre in Genres)
            {
                listablesArray.Add(genre.ToJsonToken());
            }
            token.Add("genres", listablesArray);
            return token;
        }

        public void AddGenre(MovieGenre genre)
        {
            Genres.Add(genre);
        }

        public override long GetEntityCategoryId()
        {
            return ENTITY_CATEGORY_ID;
        }

        public JObject GetListableJsonToken()
        {
            var token = ToJsonToken();
            token.Add("type", "MOVIE");

            return token;
        }
    }
}