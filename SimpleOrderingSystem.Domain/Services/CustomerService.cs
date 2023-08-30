using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Repositories;


namespace SimpleOrderingSystem.Domain.Services;

internal class CustomerService: ICustomerService
{
    private readonly ICustomersRepository _customersRepository;

    public CustomerService(ICustomersRepository customersRepository)
    {
        _customersRepository = customersRepository;
    }

    public async Task<List<Customer>> SearchCustomersAsync(string name)
    {
        var results = await _customersRepository.SearchAsync(name);

        return results.ToList();
    }

    public async Task<Customer?> GetCustomerAsync(string customerId)
    {
        if(!Guid.TryParse(customerId, out var id))
        {
            return default;
        }

        return await _customersRepository.GetAsync(id);
    }
}
