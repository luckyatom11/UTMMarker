# Diseño del Contrato de Repositorio para el Agregado "Sale" (utmMarker)

## 1. Código Fuente Completo de `ISaleRepository.cs`

Esta interfaz define el contrato para la gestión del agregado "Sale", trabajando exclusivamente con las entidades de dominio (`Sale`, `SaleDetail`) y el objeto `SaleFilter`. Todos los métodos son asíncronos y aceptan un `CancellationToken` para una resiliencia robusta.

```csharp
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
    /// Recupera una venta por su identificador único de dominio (Guid) de forma asíncrona.
    /// Incluye sus detalles de venta asociados.
    /// </summary>
    /// <param name="id">El SaleID (Guid) de la venta a recuperar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona, conteniendo la venta si se encuentra, o null.</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

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
    Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default);

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
    /// <param name="id">El SaleID (Guid) de la venta a eliminar.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

## 2. Definición del Objeto `SaleFilter`

Esta clase de tipo `record` inmutable define los criterios de filtrado para las operaciones de búsqueda de ventas. Está diseñada para ser simple y compatible con Native AOT.

```csharp
using System;

namespace utmMarker.Core.Filters;

/// <summary>
/// Representa los criterios de filtrado para la entidad Sale.
/// Diseñado como un record inmutable para compatibilidad con Native AOT.
/// </summary>
public sealed record SaleFilter
{
    /// <summary>
    /// Filtra ventas con fecha de creación posterior o igual a este valor.
    /// </summary>
    public DateTime? MinSaleDate { get; init; }

    /// <summary>
    /// Filtra ventas con fecha de creación anterior o igual a este valor.
    /// </summary>
    public DateTime? MaxSaleDate { get; init; }

    /// <summary>
    /// Filtra ventas por folio (búsqueda parcial o exacta).
    /// </summary>
    public string? Folio { get; init; }

    /// <summary>
    /// Filtra ventas por un estado específico.
    /// </summary>
    public Enums.SaleStatus? Status { get; init; }
}
```

## 3. Asesoría Arquitectónica: Impacto de Native AOT en la futura implementación de este contrato

La implementación del contrato `ISaleRepository` es un punto crítico en una arquitectura limpia optimizada para Native AOT en .NET 10. Las decisiones tomadas aquí impactarán directamente el rendimiento, el tamaño del binario y la mantenibilidad.

### Desafíos y Estrategias Clave:

1.  **Mapeo de IDs (Guid vs int) - La Inconsistencia Persistente**:
    *   **Problema**: La discrepancia entre `SaleID` (`Guid` en dominio) y `VentaID` (`int` en persistencia), así como `ProductID` (`Guid` en dominio) y `ProductoID` (`int` en persistencia) es el mayor obstáculo. El contrato del repositorio se adhiere al `Guid` del dominio. La implementación del repositorio necesitará una estrategia sólida para esta conversión.
    *   **Impacto AOT**: Si esta conversión implica `Guid.ToString()` y luego `int.Parse()` (o viceversa) sin un mapeo directo de `uniqueidentifier` a `Guid` en la base de datos, podría generar ineficiencias. Las soluciones que dependen de reflexión o de `ToString()`/`Parse()` dinámicos en tiempo de ejecución son problemáticas para AOT.
    *   **Recomendación**: La solución más limpia suele ser tener una columna `uniqueidentifier` (SQL Server) o equivalente en la base de datos para almacenar el `Guid` del dominio, y el `int` servir como clave subrogada interna. El mapeador (`SaleMapper` y el futuro `ProductMapper`) sería responsable de la traducción. **Se requiere una resolución explícita de este problema antes de una implementación robusta.**

2.  **Riesgo de Consultas N+1 al Recuperar Agregados Completos**:
    *   **Problema**: `GetAllAsync` y `GetByIdAsync` requieren la recuperación del agregado `Sale` completo, incluyendo su colección de `SaleDetail`. Una implementación ingenua podría llevar a una consulta inicial para `Sale` y luego `N` consultas separadas para cada `SaleDetail`, resultando en un problema N+1.
    *   **Impacto AOT**: Las consultas N+1 degradan el rendimiento significativamente. En un entorno Native AOT, aunque la sobrecarga de reflexión es baja, el tiempo de latencia de red y la carga en la base de datos seguirán siendo cuellos de botella.
    *   **Estrategia AOT Friendly**:
        *   **Multi-mapping de Dapper**: Utilizar las capacidades de multi-mapping de Dapper para cargar `Sale` y `SaleDetail` en una sola consulta SQL (`JOIN`). Esto es altamente eficiente y compatible con AOT.
        *   **Query divididas (`Split Query`)**: Dapper permite ejecutar múltiples consultas en una sola llamada a la base de datos y mapear los resultados por separado. Esto puede ser útil en escenarios complejos.
        *   **Source Generators (futuro)**: A medida que los Source Generators de Dapper maduren para .NET 10, podrían ofrecer soluciones de mapeo más automatizadas y AOT-compatibles para escenarios de agregados.

3.  **Filtrado Robusto con `SaleFilter` (AOT Friendly)**:
    *   **Problema**: Evitar `Expressions` genéricas en `FindAsync` es crucial para AOT, ya que requieren reflexión para su compilación en tiempo de ejecución.
    *   **Impacto AOT**: El uso de `Expression<Func<T, bool>>` directamente en el repositorio causaría que el trimmer de AOT no pudiera eliminar el código relacionado con la reflexión de `System.Linq.Expressions`, aumentando el tamaño del binario y potencialmente introduciendo errores de tiempo de ejecución.
    *   **Estrategia AOT Friendly**: El `SaleFilter` (un POCO o `record`) es la solución correcta. La implementación del repositorio construirá la cláusula `WHERE` de SQL dinámicamente a partir de las propiedades del `SaleFilter`, utilizando lógica condicional en C# en lugar de expresiones.

4.  **`IAsyncEnumerable` para Streaming y Eficiencia de Memoria**:
    *   **Impacto AOT**: El uso de `IAsyncEnumerable<T>` es intrínsecamente compatible con Native AOT. Permite que grandes conjuntos de resultados se transmitan de forma asíncrona, reduciendo el pico de uso de memoria y la presión sobre el GC. Esto es ideal para aplicaciones CLI que procesan volúmenes considerables de datos.

### Conclusión

La implementación del `ISaleRepository` debe ser cuidadosamente orquestada, prestando especial atención a la eficiencia del acceso a datos, la compatibilidad con AOT y la resolución de las inconsistencias de ID. Un enfoque basado en Dapper con multi-mapping y la construcción manual de consultas SQL será clave para lograr los objetivos de rendimiento y ligereza de un proyecto .NET 10 Native AOT.
