namespace SimpleOrderingSystem.Repositories.Http.ProviderModels;

internal record GetZipCodeCityResult
{
    public string City { get; init;} = default!;
}