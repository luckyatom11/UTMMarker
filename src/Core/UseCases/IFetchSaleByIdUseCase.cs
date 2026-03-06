using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface IFetchSaleByIdUseCase
{
    Task<Sale?> ExecuteAsync(int saleId, CancellationToken cancellationToken = default);
}
