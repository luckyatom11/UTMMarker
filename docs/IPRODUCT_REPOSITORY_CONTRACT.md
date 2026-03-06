# Diseño del Contrato de Repositorio para la Entidad "Product" (utmMarker)

## 1. Estructura de Namespaces Recomendada

Para mantener la Clean Architecture y una clara separación de responsabilidades, la estructura de namespaces recomendada es la siguiente:

*   **Dominio (Core)**:
    *   `utmMarker.Core.Entities`: Contiene las entidades de negocio puras (ej. `Product`, `Sale`, `SaleDetail`).
    *   `utmMarker.Core.Enums`: Contiene los enumerados relacionados con el dominio (ej. `SaleStatus`).
    *   `utmMarker.Core.Filters`: Contiene los objetos de criterios para filtrado (ej. `ProductFilter`).
    *   `utmMarker.Core.Repositories`: Contiene las interfaces de repositorio (contratos) para el acceso a datos (ej. `IProductRepository`).

*   **Infraestructura (Infrastructure)**:
    *   `utmMarker.Infrastructure.Models.Data`: Contiene las entidades de persistencia (ej. `ProductoEntity`, `VentaEntity`, `DetalleVentaEntity`).
    *   `utmMarker.Infrastructure.Mappers`: Contiene la lógica de mapeo bidireccional entre entidades de Dominio y de Persistencia (ej. `SaleMapper`, `ProductMapper` - este último aún pendiente).
    *   `utmMarker.Infrastructure.Repositories.Implementations`: Contendría las implementaciones concretas de los repositorios (ej. `ProductRepository`).

## 2. Código Fuente Completo de `IProductRepository.cs`

Esta interfaz define el contrato para el repositorio de productos, trabajando exclusivamente con la entidad de dominio `Product` y el objeto `ProductFilter`. Todos los métodos son asíncronos y aceptan un `CancellationToken` para resiliencia.

```csharp
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
    /// Recupera un producto por su identificador único de dominio (Guid) de forma asíncrona.
    /// </summary>
    /// <param name="id">El ProductID (Guid) del producto a recuperar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona, conteniendo el producto si se encuentra, o null.</returns>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

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
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un producto existente en el repositorio de forma asíncrona.
    /// </summary>
    /// <param name="product">El objeto Product con los datos actualizados.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza el stock de un producto específico de forma asíncrona (actualización parcial).
    /// </summary>
    /// <param name="productId">El ProductID (Guid) del producto a actualizar.</param>
    /// <param name="newStock">La nueva cantidad de stock para el producto.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task UpdateStockAsync(Guid productId, int newStock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un producto del repositorio de forma asíncrona.
    /// </summary>
    /// <param name="id">El ProductID (Guid) del producto a eliminar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

## 3. Definición de `ProductFilter.cs`

Esta clase de tipo POCO define los criterios de filtrado para las operaciones de búsqueda de productos. Está diseñada para ser simple y compatible con Native AOT.

```csharp
namespace utmMarker.Core.Filters;

/// <summary>
/// Representa los criterios de filtrado para la entidad Product.
/// Diseñado para ser simple y compatible con Native AOT.
/// </summary>
public sealed class ProductFilter
{
    /// <summary>
    /// Filtra productos por nombre (búsqueda parcial o exacta).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Filtra productos por SKU (búsqueda exacta).
    /// </summary>
    public string? SKU { get; init; }

    /// <summary>
    /// Filtra productos por marca (búsqueda parcial o exacta).
    /// </summary>
    public string? Brand { get; init; }

    /// <summary>
    /// Filtra productos con precio mayor o igual a este valor.
    /// </summary>
    public decimal? MinPrice { get; init; }

    /// <summary>
    /// Filtra productos con precio menor o igual a este valor.
    /// </summary>
    public decimal? MaxPrice { get; init; }

    /// <summary>
    /// Filtra productos con stock mayor o igual a este valor.
    /// </summary>
    public int? MinStock { get; init; }

    /// <summary>
    /// Filtra productos con stock menor o igual a este valor.
    /// </summary>
    public int? MaxStock { get; init; }
}
```

## 4. Explicación técnica: ¿Por qué `IAsyncEnumerable` es superior para una aplicación de consola en .NET 10?

`IAsyncEnumerable<T>` es una característica introducida en C# 8 (.NET Core 3.0) que permite trabajar con secuencias de datos de forma asíncrona y diferida. Para una aplicación de consola en .NET 10, especialmente cuando se busca optimización para Native AOT y bajo consumo de memoria, `IAsyncEnumerable<T>` ofrece ventajas significativas sobre otras alternativas:

1.  **Streaming de Datos (Lazy Loading)**: A diferencia de `Task<List<T>>` (que carga todos los resultados en memoria antes de devolverlos), `IAsyncEnumerable<T>` permite un procesamiento por lotes o "streaming". Los elementos se recuperan y procesan uno a uno (o en pequeños bloques) a medida que son solicitados por el consumidor, reduciendo drásticamente el uso de memoria para grandes conjuntos de datos. Esto es crucial en aplicaciones CLI que pueden operar con recursos limitados o procesar volúmenes masivos de información.

2.  **Optimización de Memoria**: Al evitar cargar todo el conjunto de resultados en memoria a la vez, se minimiza la presión sobre el recolector de basura (GC), lo que conduce a una ejecución más suave, menos pausas por GC y, en última instancia, un menor consumo de memoria. Esto es directamente beneficioso para el objetivo de una aplicación ligera optimizada para .NET 10.

3.  **Compatibilidad con Native AOT**: `IAsyncEnumerable<T>` es una característica del lenguaje y del runtime estándar de .NET. Su implementación no depende de reflexión o de generación de código dinámico, lo que la hace perfectamente compatible con el proceso de compilación Native AOT. Esto asegura que la optimización del tamaño del binario y el rendimiento en tiempo de ejecución se mantengan.

4.  **Respuesta Interactiva**: En aplicaciones de consola que interactúan con el usuario o procesan datos en tiempo real, el streaming asíncrono permite mostrar resultados a medida que están disponibles, mejorando la percepción de reactividad de la aplicación.

5.  **Patrón de Consumo Claro**: Se consume fácilmente con un bucle `await foreach`, lo que simplifica la lógica del lado del cliente y promueve un código más limpio y expresivo para el manejo de colecciones asíncronas.

**En resumen**: Al adoptar `IAsyncEnumerable<T>`, `utmMarker` puede manejar grandes volúmenes de datos de manera eficiente, con una huella de memoria reducida y sin comprometer la compatibilidad con las optimizaciones de Native AOT de .NET 10, lo que resulta en una aplicación de consola más robusta y de mayor rendimiento.

---
**NOTA IMPORTANTE SOBRE INCONSISTENCIAS DE ID:**
Tal como se ha documentado en `docs/PRODUCT_MAPPER_INCONSISTENCY.md` y reiterado en `docs/SALE_MAPPER.md`, existe una **inconsistencia crítica** en el mapeo de IDs entre el dominio (`Product.ProductID` - `Guid`, `Sale.SaleID` - `Guid`) y la capa de persistencia (`ProductoEntity.ProductoID` - `int`, `VentaEntity.VentaID` - `int`).

La interfaz `IProductRepository` ha sido diseñada para trabajar con `Guid` como identificador de dominio, pero la implementación subyacente del repositorio y del mapeador requerirá una estrategia clara para la conversión entre `Guid` y `int`. Hasta que esta inconsistencia se resuelva y se implemente un `ProductMapper.cs` funcional, cualquier implementación de `IProductRepository` para la persistencia de productos será un marcador de posición.
