using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace SimpleOrderingSystem.Repositories.Http.Providers;

internal class MovieProvider: IMovieProvider
{
    internal static readonly string HttpClientName = typeof(MovieProvider).FullName!;
    private readonly IHttpClientFactory _httpClientFactory;

    public MovieProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<GetMovieApiResponse> GetMovieAsync(string apiKey, string id)
    {
        var queryString = QueryHelpers.AddQueryString("", "i", id);
        queryString = QueryHelpers.AddQueryString(queryString, "apiKey", apiKey);

        return await Client.GetFromJsonWithDetailsAsync<GetMovieApiResponse>(queryString.ToString());
    }

    public async Task<SearchMoviesApiResponse> SearchMoviesAsync(string apiKey, string title)
    {
        var queryString = QueryHelpers.AddQueryString("", "s", title);
        queryString = QueryHelpers.AddQueryString(queryString, "apiKey", apiKey);

        return await Client.GetFromJsonWithDetailsAsync<SearchMoviesApiResponse>(queryString.ToString());
    }

    #region Helpers

    private HttpClient Client => _httpClientFactory.CreateClient(HttpClientName);

    #endregion
}