using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Enums;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class UpdateSaleStatusUseCaseImpl(ISaleRepository saleRepository) : IUpdateSaleStatusUseCase
{
    public async Task ExecuteAsync(int saleId, SaleStatus newStatus, CancellationToken cancellationToken = default) // Changed Guid to int
    {
        if (saleId <= 0) // Changed from Guid.Empty to check for non-positive int
        {
            throw new ArgumentException("Sale ID must be a positive integer.", nameof(saleId));
        }

        var saleToUpdate = await saleRepository.GetByIdAsync(saleId, cancellationToken);

        if (saleToUpdate == null)
        {
            throw new InvalidOperationException($"Sale with ID {saleId} not found.");
        }

        // Business rule: Prevent updating to a status that is 'older' than current, or invalid transitions
        // For simplicity, here we just update if it's different. More complex rules would go here.
        if (saleToUpdate.Status != newStatus)
        {
            saleToUpdate.Status = newStatus;
            await saleRepository.UpdateAsync(saleToUpdate, cancellationToken);
        }
    }
}
