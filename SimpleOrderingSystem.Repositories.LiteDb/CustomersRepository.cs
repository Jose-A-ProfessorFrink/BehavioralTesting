using SimpleOrderingSystem.Repositories.LiteDB.Providers;
using SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;
using SimpleOrderingSystem.Domain.Repositories;
using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Repositories.LiteDB.Providers;

internal class CustomersRepository: ICustomersRepository
{
    private readonly ILiteDbProvider _liteDbProvider;

    public CustomersRepository(ILiteDbProvider liteDbProvider)
    {
        _liteDbProvider = liteDbProvider;
    }

    public async Task<List<Customer>> SearchAsync(string name)
    {
        var customers = await _liteDbProvider.SearchCustomersAsync(name);

        return customers
            .Select(a=>Map(a))
            .ToList();
    }

    public async Task<Customer?> GetAsync(Guid id)
    {
        var customer = await _liteDbProvider.GetCustomerAsync(id);

        return customer is null? default: Map(customer);
    }

    #region Helpers

    private Customer Map(CustomerDataModel customer)
    {
        return new()
        {
            Id = customer.Id,
            Name = customer.Name,
            DateHired = customer.DateHired,
            DateOfBirth = customer.DateOfBirth,
            AnnualSalary = customer.AnnualSalary
        };
    }

    #endregion
}
