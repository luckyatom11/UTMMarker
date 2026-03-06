using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface IRetrieveAllProductsUseCase
{
    IAsyncEnumerable<Product> ExecuteAsync(CancellationToken cancellationToken = default);
}
