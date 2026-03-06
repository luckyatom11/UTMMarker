using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Filters;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class FetchSalesByFilterUseCaseImpl(ISaleRepository saleRepository) : IFetchSalesByFilterUseCase
{
    public IAsyncEnumerable<Sale> ExecuteAsync(SaleFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter, nameof(filter));

        // Basic validation on filter properties can be added here if needed
        // For example: if (filter.MinSaleDate > filter.MaxSaleDate) throw new ArgumentException(...)

        return saleRepository.FindAsync(filter, cancellationToken);
    }
}
