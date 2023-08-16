namespace SimpleOrderingSystem.Domain.Models;

public class ZipCodeInformation
{
    public string Code { get; init;} = default!;
    public string State { get; init;} = default!;
    public string[] Cities { get; init;} = default!;
}