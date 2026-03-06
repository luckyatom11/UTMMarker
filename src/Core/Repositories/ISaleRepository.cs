using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Filters;

namespace utmMarker.Core.Repositories;

/// <summary>
/// Contrato de repositorio para la gestión del agregado de ventas (Sale).
/// Define las operaciones de acceso a datos para ventas y sus detalles,
/// trabajando exclusivamente con el dominio y optimizado para Native AOT.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Recupera todas las ventas de forma asíncrona, permitiendo el streaming de datos.
    /// Incluye sus detalles de venta asociados.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Un IAsyncEnumerable de ventas.</returns>
    IAsyncEnumerable<Sale> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera una venta por su identificador único de dominio (int) de forma asíncrona.
    /// Incluye sus detalles de venta asociados.
    /// </summary>
    /// <param name="id">El SaleID (int) de la venta a recuperar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona, conteniendo la venta si se encuentra, o null.</returns>
    Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca ventas que coincidan con los criterios especificados en el filtro.
    /// Permite el streaming de datos y recupera los detalles de venta asociados.
    /// </summary>
    /// <param name="filter">Objeto con los criterios de búsqueda de ventas.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Un IAsyncEnumerable de ventas que coinciden con el filtro.</returns>
    IAsyncEnumerable<Sale> FindAsync(SaleFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva venta al repositorio de forma asíncrona.
    /// Después de la adición, el objeto Sale devuelto tendrá su identidad (SaleID) generada/actualizada.
    /// </summary>
    /// <param name="sale">El objeto Sale a agregar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona, conteniendo el objeto Sale persistido con su ID actualizado.</returns>
    Task<Sale> AddAsync(Sale sale, IEnumerable<SaleDetail> saleDetails, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una venta existente en el repositorio de forma asíncrona.
    /// Se espera que el objeto Sale contenga todos los detalles actualizados.
    /// </summary>
    /// <param name="sale">El objeto Sale con los datos actualizados (incluyendo detalles).</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una venta del repositorio de forma asíncrona.
    /// </summary>
    /// <param name="id">El SaleID (int) de la venta a eliminar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
