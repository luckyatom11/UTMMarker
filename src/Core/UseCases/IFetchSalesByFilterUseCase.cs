using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Filters;

namespace utmMarker.Core.UseCases;

public interface IFetchSalesByFilterUseCase
{
    IAsyncEnumerable<Sale> ExecuteAsync(SaleFilter filter, CancellationToken cancellationToken = default);
}
