using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class CreateSaleUseCaseImpl(ISaleRepository saleRepository) : ICreateSaleUseCase
{
    public async Task<Sale> ExecuteAsync(Sale sale, IEnumerable<SaleDetail> saleDetails, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sale, nameof(sale));
        ArgumentNullException.ThrowIfNull(saleDetails, nameof(saleDetails));

        // For new sales, SaleID should be 0. The repository will assign a generated ID.
        // If an ID is passed, it means we are trying to update an existing sale, which is not the purpose of this use case.
        if (sale.SaleID != 0)
        {
            throw new ArgumentException("Cannot create a sale with a pre-existing SaleID. SaleID must be 0 for new sales.", nameof(sale.SaleID));
        }

        // Assign sale details to the sale object using the AddSaleDetail method
        // The _saleDetails collection is initialized in the Sale constructor.
        foreach (var detail in saleDetails)
        {
            sale.AddSaleDetail(detail); // Assuming Sale.AddSaleDetail handles setting SaleID on SaleDetail
        }
        
        return await saleRepository.AddAsync(sale, saleDetails, cancellationToken);
    }
}
