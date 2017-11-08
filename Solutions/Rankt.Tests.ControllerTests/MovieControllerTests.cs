using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DataModel.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json.Linq;
using Rankt.Api.Controllers;
using Trakker.Api.Repositories.Movies;
using Xunit;

namespace Rankt.Tests.ControllerTests
{
    public class MovieControllerTests
    {
        private readonly Mock<IStringLocalizer<SharedResources>> mockStringLocaliser = new Mock<IStringLocalizer<SharedResources>>();
        private readonly Mock<IMemoryCache> mockMemoryCache = new Mock<IMemoryCache>();

        public MovieControllerTests()
        {
            object outVal;
            mockMemoryCache.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out outVal)).Returns(false);
        }

        [Fact]
        public async Task No_Movies_Found_When_Calling_Get_All_Movies()
        {
            var notFoundString = "This movie could not be found";

            mockStringLocaliser.Setup(s => s[It.IsAny<string>()])
                .Returns(() => new LocalizedString("controllers.movie.movie_not_found", notFoundString));
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(r => r.GetAllMovies()).Returns(Task.FromResult(new List<Movie>()));

            var movieController = new MovieController(mockRepo.Object, mockStringLocaliser.Object, mockMemoryCache.Object);

            var result = await movieController.GetAllMovies(null);

            ContentResult contentResult = result as ContentResult;
            
            Assert.NotEqual(contentResult, null);

            Assert.Equal((int)HttpStatusCode.NotFound, contentResult.StatusCode);

            var resultContent = JObject.Parse(contentResult.Content);

            Assert.Equal(notFoundString, ""+ resultContent["message"]);

            mockRepo.Verify(x => x.GetAllMovies(), Times.Once);
        }

        [Fact]
        public async Task Movie_Found_For_Movie_Id()
        {
            var myMovie = Movie.Instanciate("It", "Bad clown", DateTime.Now, 123, 12345, "tt123123", "posterPath",
                "backdropPath");

            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(r => r.GetById(It.Is<long>( c => c == 2))).Returns(Task.FromResult( myMovie));

            var movieController = new MovieController(mockRepo.Object, mockStringLocaliser.Object, mockMemoryCache.Object);

            var result = await movieController.GetById(2);

            ContentResult contentResult = result as ContentResult;

            Assert.NotEqual(contentResult, null);

            Assert.Equal((int)HttpStatusCode.OK, contentResult.StatusCode);

            var resultContent = JObject.Parse(contentResult.Content);

            Assert.Equal(myMovie.Name, "" + resultContent["name"]);
            Assert.Equal(myMovie.Overview, "" + resultContent["overview"]);
            Assert.Equal(myMovie.ReleaseDate.Value.Year, (int)resultContent["release_date"]);

            mockRepo.Verify(x => x.GetById(2), Times.Once);
        }
    }
}
