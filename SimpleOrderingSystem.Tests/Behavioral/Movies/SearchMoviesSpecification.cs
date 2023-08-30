
using Microsoft.Extensions.Configuration;
using Mindbody.Sales.Tests.Common.Application;
using SimpleOrderingSystem.Repositories.Http.Providers;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;

namespace SimpleOrderingSystem.Tests.Behavioral.Customers;

public class SearchMoviesSpecification : IDisposable
{
    // sut
    private readonly WebApplicationFactory _webApplicationFactory;

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

            },
            new()
            {

            }
        }
    };

    // custom application settings
    private Dictionary<string,string> _appSettings = new()
    {
        {"LiteDbConnectionString", "Blah"}
    };

    public SearchMoviesSpecification()
    {
        // given I have a web application factory
        _webApplicationFactory = WebApplicationFactory.Create((a) => a.AddInMemoryCollection(_appSettings));

        // given I mock out the movie provider and setup appropriate defaults
        _movieProviderMock = _webApplicationFactory.Mock<IMovieProvider>();
        _movieProviderMock
            .Setup(a=>a.SearchMoviesAsync(It.IsAny<string>(),It.IsAny<string>()))
            .ReturnsAsync(() => _searchMoviesResults);
    }

    [Fact(DisplayName = "Search movie should return bad request when search name is empty")]
    public async Task Test1()
    {
        // when I get a customer
        var response = await SearchMoviesAsync(string.Empty);

        // the I expect the response to be not found
        await response.ShouldBeTheFollowingModelStateValidationBadRequestAsync(
            ("name", "The Name field is required."));
    }

    #region Helpers

    public async Task<HttpResponseMessage> SearchMoviesAsync(string name)
    {
        return await _webApplicationFactory.CreateClient().GetAsync($"Movies/search?name={name}");
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
    }

    #endregion
}