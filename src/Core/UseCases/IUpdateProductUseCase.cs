using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface IUpdateProductUseCase
{
    Task ExecuteAsync(Product product, CancellationToken cancellationToken = default);
}
