using System.Collections.Generic;
using System.Linq;
using Common.Model.Movies;
using DataModel.Genres;

namespace Rankt.Api.Data
{
    public class DbInitializer
    {
        public static void Initialize(RanktContext context)
        {
            context.Database.EnsureCreated();

            if (context.Movies.Any())
            {
                return;   // DB has been seeded
            }

            var action = new MovieGenre {SourceSite = "TMDB", Name = "Action", SourceId = 28};
            var adventure = new MovieGenre {SourceSite = "TMDB", Name = "Adventure", SourceId = 12};
            var animation = new MovieGenre {SourceSite = "TMDB", Name = "Animation", SourceId = 16};

            var movieGenres = new MovieGenre[]
            {
                action, adventure, animation
            };

            foreach (var s in movieGenres)
            {
                context.MovieGenres.Add(s);
            }
            context.SaveChanges();

            var movies = new Movie[]
            {
                new Movie {Name = "The Last Jedi", Overview = "Mark hamill last role",
                    Genres = new List<MovieGenre>(){action, adventure}},
            };

            foreach (var s in movies)
            {
                context.Movies.Add(s);
            }
            context.SaveChanges();
        }
    }
}