namespace SimpleOrderingSystem.Repositories.Http.ProviderModels;

internal record GetZipCodeApiResponse
{
    public GetZipCodeResult Results {get; init;} = default!;
}
