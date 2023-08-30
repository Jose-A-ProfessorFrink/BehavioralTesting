

namespace SimpleOrderingSystem.Tests;

/// <summary>
/// Houses default values that can be used for common mock return value setups.
/// </summary>
public static class Defaults
{
    public static readonly Guid CustomerId = new Guid("c0000000-1bf2-4ef7-90cb-000000000001");
    public static readonly string MovieId = "gone with da wind, yo";
    public static readonly string MovieServiceApiKey = nameof(MovieServiceApiKey);
    public static readonly string CustomerName = "Homer Simpson";
    public static readonly DateOnly CustomerDateOfBirth = new DateOnly(1975, 10, 15);
    public static readonly DateOnly CustomerDateHired = new DateOnly(1999, 1, 15);
    public static readonly decimal CustomerAnnualSalary = 64540;
}