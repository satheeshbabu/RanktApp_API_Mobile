using System;
using Common.Model.Movies;
using NUnit.Framework;

namespace Common.Model.UnitTests
{
    
    public class MovieTests
    {
        [Test]
        public void Generate_Movie_Valid_Release_Date()
        {
            var releaseDate = new DateTime(2017, 5, 6);
            var movie = Movie.Instanciate("IT", "Evil Clown", releaseDate, 123,
                12345, "tt12344", "poster", "backdrop", DateTime.Now);

            var json = movie.ToJsonToken();

            Assert.AreEqual(releaseDate, DateTime.Parse(json["release_date"] + ""));
        }
    }
}
