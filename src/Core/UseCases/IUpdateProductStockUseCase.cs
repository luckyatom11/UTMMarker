using System;
using System.Threading;
using System.Threading.Tasks;

namespace utmMarker.Core.UseCases;

public interface IUpdateProductStockUseCase
{
    Task ExecuteAsync(int productId, int stockChange, CancellationToken cancellationToken = default);
}
