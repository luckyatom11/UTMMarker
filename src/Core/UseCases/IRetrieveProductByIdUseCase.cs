using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface IRetrieveProductByIdUseCase
{
    Task<Product?> ExecuteAsync(int productId, CancellationToken cancellationToken = default);
}
