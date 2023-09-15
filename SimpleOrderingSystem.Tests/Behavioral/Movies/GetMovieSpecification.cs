using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;

namespace SimpleOrderingSystem.Tests.Behavioral.Customers;

public class GetMovieSpecification : IDisposable
{
    // sut
    private readonly WebApplicationFactory _webApplicationFactory;

    // mocks
    private readonly Mock<IMovieProvider> _movieProviderMock;
    
    // default return objects bound to mocks
    private GetMovieApiResponse _getMovieApiResponse = new()
    {
        ImdbId= "tt1856101",
        Title= "Blade Runner 2049",
        Year= "2017",
        Rated= "R",
        Released= "06 Oct 2017",
        Plot= "a plot",
        Poster= "https://m.media-amazon.com/images/M/MV5BNzA1Njg4NzYxOV5BMl5BanBnXkFtZTgwODk5NjU3MzI@._V1_SX300.jpg",
        Metascore= "81",
        ImdbRating= "8.0",
        Type= "movie",
        Response= "True",
        Error = default
    };

    // custom application settings
    private Dictionary<string,string> _appSettings = new()
    {
        {"OmdbApiKey", Defaults.MovieServiceApiKey}
    };

    public GetMovieSpecification()
    {
        // given I have a web application factory
        _webApplicationFactory = WebApplicationFactory.Create((a) => a.AddInMemoryCollection(_appSettings));

        // given I mock out the movie provider and setup appropriate defaults
        _movieProviderMock = _webApplicationFactory.Mock<IMovieProvider>();
        _movieProviderMock
            .Setup(a=>a.GetMovieAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(() => _getMovieApiResponse);
    }

    [Fact(DisplayName = "Get movie should return not found when movie id is invalid")]
    public async Task Test1()
    {
        // given I set the movie response to invalid
        _getMovieApiResponse = _getMovieApiResponse with { Response = "False" } ;

        // when I get a customer
        var response = await GetMovieAsync(Defaults.MovieId);

        // the I expect the response to be not found
        response.ShouldBeNotFound();
    }

    [Fact(DisplayName = "Get movie should return correct movie data from service and invoke service with correct parameters")]
    public async Task Test3()
    {
        // when I get a customer
        var response = await GetMovieAsync(Defaults.MovieId);

        // then I expect the following 
        await response.ShouldBeOkWithResponseAsync(new TestMovieViewModel
        {
            Id= "tt1856101",
            Title= "Blade Runner 2049",
            Year= "2017",
            Rating= "R",
            DateReleased= "06 Oct 2017",
            PlotDescription= "a plot",
            PosterUrl= "https://m.media-amazon.com/images/M/MV5BNzA1Njg4NzYxOV5BMl5BanBnXkFtZTgwODk5NjU3MzI@._V1_SX300.jpg",
            Metascore= "81",
            ImdbRating= "8.0",
            Type= "movie",      
        });

        // then I expect the provider to have been called with the following
        _movieProviderMock.Verify(a=>a.GetMovieAsync( Defaults.MovieServiceApiKey, Defaults.MovieId), Times.Once());
    }

    #region Helpers

    public async Task<HttpResponseMessage> GetMovieAsync(string movieId)
    {
        return await _webApplicationFactory.CreateClient().GetAsync($"Movies/{movieId}");
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
    }

    #endregion
}