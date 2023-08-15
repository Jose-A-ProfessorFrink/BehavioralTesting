using SimpleOrderingSystem.DataModels;

namespace SimpleOrderingSystem.Repositories;

internal interface ICustomersRepository
{
    Task<List<CustomerDataModel>> SearchAsync(string name);

    Task<CustomerDataModel?> GetAsync(Guid id);
}
