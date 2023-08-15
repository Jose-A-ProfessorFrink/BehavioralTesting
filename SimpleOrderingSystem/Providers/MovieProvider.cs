using SimpleOrderingSystem.DataModels;
using System.Net;
using SimpleOrderingSystem.Extensions;

namespace SimpleOrderingSystem.Providers;

internal class MovieProvider: IMovieProvider
{
    internal static readonly string HttpClientName = typeof(MovieProvider).FullName!;
    private readonly IHttpClientFactory _httpClientFactory;

    public MovieProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<GetMovieApiResponseDataModel> GetMovieAsync(string apiKey, string id)
    {
        var queryString = QueryString.Create(new[]
        {
            new KeyValuePair<string, string?>("i", id),
            new KeyValuePair<string, string?>("apiKey", apiKey)
        });

        return await Client.GetFromJsonWithDetailsAsync<GetMovieApiResponseDataModel>(queryString.ToString());
    }

    public async Task<SearchMoviesApiResponseDataModel> SearchMoviesAsync(string apiKey, string title)
    {
        var queryString = QueryString.Create(new[]
        {
            new KeyValuePair<string, string?>("s", title),
            new KeyValuePair<string, string?>("apiKey", apiKey)
        });

        return await Client.GetFromJsonWithDetailsAsync<SearchMoviesApiResponseDataModel>(queryString.ToString());
    }

    #region Helpers

    private HttpClient Client => _httpClientFactory.CreateClient(HttpClientName);

    #endregion
}