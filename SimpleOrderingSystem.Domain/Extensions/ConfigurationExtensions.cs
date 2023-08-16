
using Microsoft.Extensions.Configuration;
namespace SimpleOrderingSystem.Domain.Extensions;

/// <summary>
/// Configuration extensions.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets a required value.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetRequiredValue(this IConfiguration c, string key)
    {
        var value = c.GetValue<string>(key);

        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Configuration is missing a value for {key}");
        }

        return value;
    }
}
