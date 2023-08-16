namespace SimpleOrderingSystem.Repositories.Http.ProviderModels;

internal record GetZipCodeResult
{
    public string? Error { get; set; }
    public string Zip { get; init;} = default!;
    public string State { get; init;} = default!;
    public List<GetZipCodeCityResult> Cities { get; init;} = new List<GetZipCodeCityResult>();
}