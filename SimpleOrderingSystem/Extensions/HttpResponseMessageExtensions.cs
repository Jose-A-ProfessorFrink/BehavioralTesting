

namespace SimpleOrderingSystem.Extensions;

/// <summary>
/// Configuration extensions.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Gets a required value.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static async Task EnsureSuccessStatusCodeWithDetailsAsync(this HttpResponseMessage responseMessage)
    {
        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            var message = $"Http invocation returned non success status code '{responseMessage.StatusCode}'. \r\n " 
                + $"Received following response from service: \r\n \r\n {await responseMessage.Content.ReadAsStringAsync()}";
            throw new Exception(message, ex);
        }
    }
}