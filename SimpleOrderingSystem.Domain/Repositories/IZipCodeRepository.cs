using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Domain.Repositories;

public interface IZipCodeRepository
{
    Task<ZipCodeInformation?> GetZipCodeInformationAsync(string code);
}
