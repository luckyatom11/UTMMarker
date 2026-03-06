using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;

namespace utmMarker.Core.UseCases;

public interface ICreateSaleUseCase
{
    Task<Sale> ExecuteAsync(Sale sale, IEnumerable<SaleDetail> saleDetails, CancellationToken cancellationToken = default);
}
