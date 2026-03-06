using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class RetrieveProductByIdUseCaseImpl(IProductRepository productRepository) : IRetrieveProductByIdUseCase
{
    public async Task<Product?> ExecuteAsync(int productId, CancellationToken cancellationToken = default)
    {
        if (productId <= 0) // Changed from Guid.Empty to check for non-positive int
        {
            throw new ArgumentException("Product ID must be a positive integer.", nameof(productId));
        }

        return await productRepository.GetByIdAsync(productId, cancellationToken);
    }
}
