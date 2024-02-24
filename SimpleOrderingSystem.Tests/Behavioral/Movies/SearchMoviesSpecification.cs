
using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;

namespace SimpleOrderingSystem.Tests.Behavioral.Customers;

public class SearchMoviesSpecification : IClassFixture<WebApplicationFactoryFixture>
{
    // sut
    private readonly HttpClient _httpClient;

    // mocks
    private readonly Mock<IMovieProvider> _movieProviderMock;
    
    // default return objects bound to mocks
    private SearchMoviesApiResponse _searchMoviesResults = new SearchMoviesApiResponse()
    { 
        Response = "True",
        Error = default,
        Search = new List<SearchMoviesResult>()
        {
            new()
            {
                Title= "Blade Runner",
                Year= "1982",
                ImdbId= "tt0083658",
                Type= "movie",
                Poster= "https://m.media-amazon.com/images/M/MV5BNzQzMzJhZTEtOWM4NS00MTdhLTg0YjgtMjM4MDRkZjUwZDBlXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_SX300.jpg"
            },
            new()
            {
                Title= "Blade Runner 2049",
                Year= "2017",
                ImdbId= "tt1856101",
                Type= "movie",
                Poster= "https://m.media-amazon.com/images/M/MV5BNzA1Njg4NzYxOV5BMl5BanBnXkFtZTgwODk5NjU3MzI@._V1_SX300.jpg"
            }
        }
    };

    // custom application settings
    private Dictionary<string,string?> _appSettings = new()
    {
        {"OmdbApiKey", Defaults.MovieServiceApiKey}
    };

    public SearchMoviesSpecification(WebApplicationFactoryFixture webApplicationFactory)
    {
        // given I have a web application factory
        webApplicationFactory.Setup(_appSettings);

        // given I mock out the movie provider and setup appropriate defaults
        _movieProviderMock = webApplicationFactory.Mock<IMovieProvider>();
        _movieProviderMock
            .Setup(a=>a.SearchMoviesAsync(It.IsAny<string>(),It.IsAny<string>()))
            .ReturnsAsync(() => _searchMoviesResults);

        // given I have an HttpClient
        _httpClient = webApplicationFactory.CreateClient();
    }

    [Fact(DisplayName = "Search movie should return bad request when search name is empty")]
    public async Task Test1()
    {
        // when I get a search
        var response = await SearchMoviesAsync(string.Empty);

        // the I expect the response to be not found
        await response.ShouldBeTheFollowingModelStateValidationBadRequestAsync(
            ("name", "The Name field is required."));
    }

    [Fact(DisplayName = "Search movie should return bad request when search name is too broad")]
    public async Task Test2()
    {
        // given I have too many results
        _searchMoviesResults.Response = "False";
        _searchMoviesResults.Error = "Too many results.";

        // when I get a search
        var response = await SearchMoviesAsync("something");

        // the I expect the response to be not found
        await response.ShouldBeTheFollowingServiceExceptionBadRequestAsync(
            "SearchMovieRequestTooBroad",
            "You need to widen your search because the supplied search parameter was too broad.");
    }

    [Fact(DisplayName = "Search movie should return ok with empty list when no search results are found")]
    public async Task Test3()
    {
        // given I have no movies
        _searchMoviesResults.Response = "False";

        // when I get a search
        var response = await SearchMoviesAsync("something");

        // then I expect the response to be not found
        await response.ShouldBeOkWithResponseAsync(new TestMovieSearchResponseViewModel());
    }

    [Fact(DisplayName = "Search movie should return movie result information correctly when search results are found")]
    public async Task Test4()
    {
        // when I get a search
        var response = await SearchMoviesAsync("blade");

        // then I expect the response to be not found
        await response.ShouldBeOkWithResponseAsync(new TestMovieSearchResponseViewModel()
        {
            Movies = new List<TestMovieSearchViewModel>()
            {
                new()
                {
                    Title= "Blade Runner",
                    Year= "1982",
                    Id= "tt0083658",
                    Type= "movie",
                    PosterUrl= "https://m.media-amazon.com/images/M/MV5BNzQzMzJhZTEtOWM4NS00MTdhLTg0YjgtMjM4MDRkZjUwZDBlXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_SX300.jpg"
                },
                new()
                {
                    Title= "Blade Runner 2049",
                    Year= "2017",
                    Id= "tt1856101",
                    Type= "movie",
                    PosterUrl= "https://m.media-amazon.com/images/M/MV5BNzA1Njg4NzYxOV5BMl5BanBnXkFtZTgwODk5NjU3MzI@._V1_SX300.jpg"
                }                
            }        
        });

        // then the api should have been called with the following
        _movieProviderMock
            .Verify(a=>a.SearchMoviesAsync(Defaults.MovieServiceApiKey, "blade"));
    }

    #region Helpers

    public async Task<HttpResponseMessage> SearchMoviesAsync(string name)
    {
        return await _httpClient.GetAsync($"Movies/search?name={name}");
    }

    #endregion
}