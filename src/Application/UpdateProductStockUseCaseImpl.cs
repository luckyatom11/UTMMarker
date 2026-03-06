using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class UpdateProductStockUseCaseImpl(IProductRepository productRepository, IRetrieveProductByIdUseCase retrieveProductByIdUseCase) : IUpdateProductStockUseCase
{
    public async Task ExecuteAsync(int productId, int stockChange, CancellationToken cancellationToken = default)
    {
        if (productId <= 0)
        {
            throw new ArgumentException("Product ID must be a positive integer.", nameof(productId));
        }

        // Retrieve the current product to get its stock
        var existingProduct = await retrieveProductByIdUseCase.ExecuteAsync(productId, cancellationToken);
        if (existingProduct == null)
        {
            throw new InvalidOperationException($"Product with ID {productId} not found for stock update.");
        }

        int currentStock = existingProduct.Stock;
        int finalStock = currentStock + stockChange;

        if (finalStock < 0)
        {
            throw new InvalidOperationException($"Cannot update stock. Insufficient stock for product {existingProduct.Name}. Current: {currentStock}, Attempted change: {stockChange}. Resulting stock would be negative.");
        }

        await productRepository.UpdateStockAsync(productId, finalStock, cancellationToken);
    }
}
