using SimpleOrderingSystem.Domain.Extensions;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Repositories.Http.Providers;
using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace SimpleOrderingSystem.Repositories.Http;

internal class ZipCodeRepository:IZipCodeRepository
{
    private readonly IZipCodeProvider _zipCodeProvider;
    private readonly IOptions<SimpleOrderingSystemOptions> _configuration;

    public ZipCodeRepository(IZipCodeProvider zipCodeProvider,  IOptions<SimpleOrderingSystemOptions> configuration)
    {
        _zipCodeProvider = zipCodeProvider;
        _configuration = configuration;
    }

    public async Task<ZipCodeInformation?> GetZipCodeInformationAsync(string code)
    {
        var apiResponse = await _zipCodeProvider.GetZipCodeAsync(ApiKey, code);

        if(apiResponse.Results.Error is not null)
        {
            if(apiResponse.Results.Error.Contains("is not a valid ZIP code.", StringComparison.OrdinalIgnoreCase))  
            {
                return default;
            }

            throw new Exception($"Encountered unexpected error from service: {apiResponse.Results.Error}");
        }
        
        return Map(apiResponse);
    }

    #region Helpers

    private string ApiKey => _configuration.Value.ZipwiseApiKey!;

    private ZipCodeInformation Map(GetZipCodeApiResponse response)
    {
        return new()
        {
            Code = response.Results.Zip,
            State = response.Results.State,
            Cities = response.Results.Cities.Select(a=>a.City).ToArray()
        };
    }

    #endregion
}
