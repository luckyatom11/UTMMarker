using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public class RetrieveAllCustomersUseCaseImpl(ICustomerRepository customerRepository) : IRetrieveAllCustomersUseCase
{
    public async Task<IEnumerable<Customer>> ExecuteAsync()
    {
        return await customerRepository.GetAllAsync();
    }
}
