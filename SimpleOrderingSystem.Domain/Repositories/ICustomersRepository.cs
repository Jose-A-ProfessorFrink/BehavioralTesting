using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Domain.Repositories;

public interface ICustomersRepository
{
    Task<List<Customer>> SearchAsync(string name);

    Task<Customer?> GetAsync(Guid id);
}
