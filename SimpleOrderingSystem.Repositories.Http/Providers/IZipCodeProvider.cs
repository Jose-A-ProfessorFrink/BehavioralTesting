using SimpleOrderingSystem.Repositories.Http.ProviderModels;

namespace SimpleOrderingSystem.Repositories.Http.Providers;

internal interface IZipCodeProvider
{
    Task<GetZipCodeApiResponse> GetZipCodeAsync(string apiKey, string code);
}