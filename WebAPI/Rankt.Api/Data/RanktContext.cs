using Common.Model.Movies;
using DataModel.Genres;
using Microsoft.EntityFrameworkCore;

namespace Rankt.Api.Data
{
    public class RanktContext : DbContext
    {
        public RanktContext(DbContextOptions<RanktContext> options) : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().ToTable("tblMovie");
            modelBuilder.Entity<MovieGenre>().ToTable("tblMovieGenre");
        }
    }
}