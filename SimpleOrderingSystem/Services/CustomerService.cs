using SimpleOrderingSystem.Models;
using SimpleOrderingSystem.Repositories;
using SimpleOrderingSystem.DataModels;
using Microsoft.VisualBasic;


namespace SimpleOrderingSystem.Services;

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

        return results.Select(a => Map(a)).ToList();
    }

    public async Task<Customer?> GetCustomerAsync(string customerId)
    {
        if(!Guid.TryParse(customerId, out var id))
        {
            return default;
        }

        var result = await _customersRepository.GetAsync(id);

        if(result is null)
        {
            return default;
        }

        return Map(result);
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
