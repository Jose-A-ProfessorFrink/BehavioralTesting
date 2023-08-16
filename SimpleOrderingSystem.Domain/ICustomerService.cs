using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Domain;

public interface ICustomerService
{
    Task<List<Customer>> SearchCustomersAsync(string name);

    Task<Customer?> GetCustomerAsync(string id);
}