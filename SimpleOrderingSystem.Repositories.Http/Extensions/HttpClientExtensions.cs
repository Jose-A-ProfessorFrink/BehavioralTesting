using System.Net.Http.Json;

namespace SimpleOrderingSystem.Extensions;

/// <summary>
/// Http client extensions.
/// </summary>
public static class HttpClientExtensions
{
    public static async Task<T> GetFromJsonWithDetailsAsync<T>(this HttpClient client, string requestUri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        var response = await client.SendAsync(request);

        await response.EnsureSuccessStatusCodeWithDetailsAsync();

        var result = await response.Content.ReadFromJsonAsync<T>();

        if(result is null)
        {
            throw new Exception("Unable to materialize Json result from response content.");
        }

        return result;
    }
}





