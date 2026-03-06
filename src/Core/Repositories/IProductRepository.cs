using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Filters;

namespace utmMarker.Core.Repositories;

/// <summary>
/// Contrato de repositorio para la gestión de entidades Product.
/// Define las operaciones de acceso a datos para productos, trabajando exclusivamente con el dominio.
/// Compatible con Native AOT.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Recupera todos los productos de forma asíncrona, permitiendo el streaming de datos.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Un IAsyncEnumerable de productos.</returns>
    IAsyncEnumerable<Product> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera un producto por su identificador único de dominio (int) de forma asíncrona.
    /// </summary>
    /// <param name="id">El ProductID (int) del producto a recuperar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona, conteniendo el producto si se encuentra, o null.</returns>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca productos que coincidan con los criterios especificados en el filtro.
    /// Permite el streaming de datos para optimizar el uso de memoria.
    /// </summary>
    /// <param name="filter">Objeto con los criterios de búsqueda de productos.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Un IAsyncEnumerable de productos que coinciden con el filtro.</returns>
    IAsyncEnumerable<Product> FindAsync(ProductFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo producto al repositorio de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product a agregar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona, conteniendo el producto agregado con su ID generado.</returns>
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un producto existente en el repositorio de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product con los datos actualizados.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona, conteniendo el producto actualizado.</returns>
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza el stock de un producto específico de forma asíncrona (actualización parcial).
    /// </summary>
    /// <param name="productId">El ProductID (int) del producto a actualizar.</param>
    /// <param name="newStock">La nueva cantidad de stock para el producto.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un producto del repositorio de forma asíncrona.
    /// </summary>
    /// <param name="id">El ProductID (int) del producto a eliminar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
