using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class FetchAllSalesUseCaseImpl(ISaleRepository saleRepository) : IFetchAllSalesUseCase
{
    public IAsyncEnumerable<Sale> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return saleRepository.GetAllAsync(cancellationToken);
    }
}
