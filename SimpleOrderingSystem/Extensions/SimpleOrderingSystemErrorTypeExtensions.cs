using SimpleOrderingSystem.Models;
using System.ComponentModel;

namespace SimpleOrderingSystem.Extensions;

public static class SimpleOrderingSystemErrorTypeExtensions
{
    public static string? GetDescription(this SimpleOrderingSystemErrorType errorType)
    {
        var fieldInfo = errorType.GetType().GetField(errorType.ToString());

        var attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        return attributes?.FirstOrDefault()?.Description;
    }
}