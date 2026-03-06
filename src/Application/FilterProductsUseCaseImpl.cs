using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Filters;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class FilterProductsUseCaseImpl(IProductRepository productRepository) : IFilterProductsUseCase
{
    public IAsyncEnumerable<Product> ExecuteAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter, nameof(filter));

        // Basic validation on filter properties can be added here if needed
        // For example: if (filter.MinPrice > filter.MaxPrice) throw new ArgumentException(...)

        return productRepository.FindAsync(filter, cancellationToken);
    }
}
