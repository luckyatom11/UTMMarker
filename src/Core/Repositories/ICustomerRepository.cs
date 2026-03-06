using utmMarker.Core.Entities;

namespace utmMarker.Core.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(string email);
    Task AddAsync(Customer customer);
    Task<IEnumerable<Customer>> GetAllAsync();
}
