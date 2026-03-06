using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface IFetchAllSalesUseCase
{
    IAsyncEnumerable<Sale> ExecuteAsync(CancellationToken cancellationToken = default);
}
