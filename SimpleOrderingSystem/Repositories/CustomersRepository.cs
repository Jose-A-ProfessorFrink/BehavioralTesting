using System.Reflection;
using SimpleOrderingSystem.DataModels;
using SimpleOrderingSystem.Providers;

namespace SimpleOrderingSystem.Repositories;

internal class CustomersRepository: ICustomersRepository
{
    private readonly ILiteDbProvider _liteDbProvider;

    public CustomersRepository(ILiteDbProvider liteDbProvider)
    {
        _liteDbProvider = liteDbProvider;
    }

    public Task<List<CustomerDataModel>> SearchAsync(string name)
    {
        var customers = _liteDbProvider.GetCustomersCollection();

        return Task.FromResult(customers
            .Query()
            .Where(a=>a.Name.Contains(name))
            .OrderBy(a=>a.Name)
            .ToList());
    }

    public Task<CustomerDataModel?> GetAsync(Guid id)
    {
        var customers = _liteDbProvider.GetCustomersCollection();

        return Task.FromResult<CustomerDataModel?>(customers
            .Query()
            .Where(a=>a.Id == id)
            .SingleOrDefault());
    }
}
