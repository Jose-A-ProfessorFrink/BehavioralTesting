using SimpleOrderingSystem.Models;

namespace SimpleOrderingSystem.Services;

public interface ICustomerService
{
    Task<List<Customer>> SearchCustomersAsync(string name);

    Task<Customer?> GetCustomerAsync(string id);
}