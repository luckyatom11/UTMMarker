using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Enums; // Required for SaleStatus

namespace utmMarker.Core.UseCases;

public interface IUpdateSaleStatusUseCase
{
    Task ExecuteAsync(int saleId, SaleStatus newStatus, CancellationToken cancellationToken = default);
}
