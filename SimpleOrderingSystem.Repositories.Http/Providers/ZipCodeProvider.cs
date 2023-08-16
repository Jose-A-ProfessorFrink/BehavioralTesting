using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace SimpleOrderingSystem.Repositories.Http.Providers;

internal class ZipCodeProvider: IZipCodeProvider
{
    internal static readonly string HttpClientName = typeof(ZipCodeProvider).FullName!;
    private readonly IHttpClientFactory _httpClientFactory;

    public ZipCodeProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<GetZipCodeApiResponse> GetZipCodeAsync(string apiKey, string code)
    {
        var queryString = QueryHelpers.AddQueryString("", "key", apiKey);
        queryString = QueryHelpers.AddQueryString(queryString, "zip", code);
        queryString = QueryHelpers.AddQueryString(queryString, "format", "json");

        return await Client.GetFromJsonWithDetailsAsync<GetZipCodeApiResponse>(queryString.ToString());
    }

    #region Helpers

    private HttpClient Client => _httpClientFactory.CreateClient(HttpClientName);

    #endregion
}