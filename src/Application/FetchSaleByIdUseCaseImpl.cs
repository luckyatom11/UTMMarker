using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class FetchSaleByIdUseCaseImpl(ISaleRepository saleRepository) : IFetchSaleByIdUseCase
{
    public async Task<Sale?> ExecuteAsync(int saleId, CancellationToken cancellationToken = default)
    {
        if (saleId <= 0) // Changed from Guid.Empty to check for non-positive int
        {
            throw new ArgumentException("Sale ID must be a positive integer.", nameof(saleId));
        }

        return await saleRepository.GetByIdAsync(saleId, cancellationToken);
    }
}
