using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class UpdateProductUseCaseImpl(IProductRepository productRepository) : IUpdateProductUseCase
{
    public async Task ExecuteAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product, nameof(product));

        if (product.ProductID <= 0) // Changed from Guid.Empty to check for non-positive int
        {
            throw new ArgumentException("Product ID must be a positive integer for update operation.", nameof(product.ProductID));
        }

        // Optional: Check if product exists before attempting to update
        // var existingProduct = await productRepository.GetByIdAsync(product.ProductID, cancellationToken);
        // if (existingProduct == null)
        // {
        //     throw new InvalidOperationException($"Product with ID {product.ProductID} not found.");
        // }

        // Further business validation can go here

        await productRepository.UpdateAsync(product, cancellationToken);
    }
}
