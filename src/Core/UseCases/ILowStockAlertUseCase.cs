using System.Collections.Generic;
using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface ILowStockAlertUseCase
{
    IAsyncEnumerable<Product> ExecuteAsync(int threshold);
}
