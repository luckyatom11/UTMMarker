using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface IRetrieveAllCustomersUseCase
{
    Task<IEnumerable<Customer>> ExecuteAsync();
}
