using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Filters;

namespace utmMarker.Core.UseCases;

public interface IFilterProductsUseCase
{
    IAsyncEnumerable<Product> ExecuteAsync(ProductFilter filter, CancellationToken cancellationToken = default);
}
