using System.Collections.Generic;
using System.Threading;
using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class RetrieveAllProductsUseCaseImpl(IProductRepository productRepository) : IRetrieveAllProductsUseCase
{
    public IAsyncEnumerable<Product> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return productRepository.GetAllAsync(cancellationToken);
    }
}
