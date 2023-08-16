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
    
    public async Task<GetMovieApiResponseDataModel> GetMovieAsync(string apiKey, string id)
    {
        var queryString = QueryHelpers.AddQueryString("", "i", id);
        queryString = QueryHelpers.AddQueryString(queryString, "apiKey", apiKey);

/*
        var queryString = QueryString.Create(new[]
        {
            new KeyValuePair<string, string?>("i", id),
            new KeyValuePair<string, string?>("apiKey", apiKey)
        });
        */

        return await Client.GetFromJsonWithDetailsAsync<GetMovieApiResponseDataModel>(queryString.ToString());
    }

    public async Task<SearchMoviesApiResponseDataModel> SearchMoviesAsync(string apiKey, string title)
    {
        var queryString = QueryHelpers.AddQueryString("", "s", title);
        queryString = QueryHelpers.AddQueryString(queryString, "apiKey", apiKey);
        /*
        var queryString = QueryString.Create(new[]
        {
            new KeyValuePair<string, string?>("s", title),
            new KeyValuePair<string, string?>("apiKey", apiKey)
        });
        */

        return await Client.GetFromJsonWithDetailsAsync<SearchMoviesApiResponseDataModel>(queryString.ToString());
    }

    #region Helpers

    private HttpClient Client => _httpClientFactory.CreateClient(HttpClientName);

    #endregion
}