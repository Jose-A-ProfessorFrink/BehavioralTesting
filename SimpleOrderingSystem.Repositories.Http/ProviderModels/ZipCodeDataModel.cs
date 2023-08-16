namespace SimpleOrderingSystem.Repositories.Http.ProviderModels;

internal record ZipCodeDataModel
{
    public string Zip { get; init;} = default!;
    public string State { get; init;} = default!;
    public string County { get; init;} = default!;
    public string TimeZone { get; init;} = default!;
}
