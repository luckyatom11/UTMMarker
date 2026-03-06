using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface ICreateProductUseCase
{
    Task<Product> ExecuteAsync(Product product, CancellationToken cancellationToken = default);
}
