using System;
using System.Collections.Generic;
using DataModel.Attributes;
using DataModel.Base;
using DataModel.Genres;
using Newtonsoft.Json.Linq;

namespace Common.Model.Movies
{
    public class Movie 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int RunTime { get; set; }
        public long TmdbId { get; set; }
        public string ImdbId { get; set; }
        public string TmdbPosterPath { get; set; }
        public string TmdbBackdropPath { get; set; }
        public DateTime DateUpdated { get; set; }

        public ICollection<MovieGenre> Genres { get; set; }
    }
}