# Diseño de Mapeador de Ventas y Detalles para utmMarker (AOT-Compatible)

## Advertencia Crítica: Inconsistencia en el Mapeo de IDs

A pesar de que se ha procedido con la implementación del `SaleMapper.cs`, es **IMPRESCINDIBLE** destacar que las inconsistencias en el tipo de `ProductID` (Guid en dominio, int en persistencia) y `SaleID` (Guid en dominio, int en persistencia) aún **NO HAN SIDO RESUELTAS**.

El código generado en `SaleMapper.cs` incluye **marcadores de posición** y asunciones para estos IDs. Para una solución robusta y funcional, se requiere una estrategia clara para la conversión bidireccional entre `Guid` y `int` para las claves primarias (`ProductID` y `SaleID`). Esto podría implicar:

*   Añadir columnas `Guid` explícitas en las entidades de persistencia (`ProductoEntity`, `VentaEntity`).
*   Implementar un servicio de traducción de IDs.
*   Alinear los tipos de IDs en ambas capas.

**El `SaleMapper.cs` no es completamente funcional hasta que esta discrepancia fundamental sea abordada y resuelta.**

---

## 1. Árbol de Directorios de la Capa de Infraestructura Actualizado

```
C:\Programacion\utmMarker
├───Program.cs
├───utmMarker.csproj
├───utmMarker.sln
├───bin
├───db
├───docs
│   ├───ARCHITECTURE.md
│   ├───DOMAIN_ENTITIES.md
│   ├───INFRASTRUCTURE_MODELS.md
│   └───PRODUCT_MAPPER_INCONSISTENCY.md
├───obj
├───prompts
├───scripts
└───src
    ├───Core
    │   ├───Entities
    │   │   ├───Product.cs
    │   │   ├───Sale.cs
    │   │   └───SaleDetail.cs
    │   └───Enums
    │       └───SaleStatus.cs
    ├───Infrastructure
    │   ├───Mappers
    │   │   └───SaleMapper.cs
    │   └───Models
    │       └───Data
    │           ├───DetalleVentaEntity.cs
    │           ├───ProductoEntity.cs
    │           └───VentaEntity.cs
```

## 2. Código Fuente Completo de `SaleMapper.cs`

Esta clase estática utiliza métodos de extensión de C# 14 para proporcionar un mapeo bidireccional entre las entidades de dominio (`Sale`, `SaleDetail`) y las entidades de persistencia (`VentaEntity`, `DetalleVentaEntity`). Se han incluido comentarios técnicos detallados.

```csharp
using utmMarker.Core.Entities;
using utmMarker.Core.Enums;
using utmMarker.Infrastructure.Models.Data;
using System.Linq; // Necesario para .Sum() y .ToList()
using System.Collections.Generic; // Necesario para List<T>

namespace utmMarker.Infrastructure.Mappers;

public static class SaleMapper
{
    /// <summary>
    /// Transforma una entidad de persistencia VentaEntity a una entidad de dominio Sale.
    /// NOTA CRÍTICA: El mapeo de VentaID (int) a SaleID (Guid) es un marcador de posición.
    /// Se asume que el VentaID de la base de datos es un ID autoincremental y SaleID un Guid de negocio.
    /// SIN UNA COLUMNA GUID CORRESPONDIENTE EN VENTAENTITY, ESTE MAPEO ES INCOMPLETO.
    /// </summary>
    /// <param name="entity">La VentaEntity a transformar.</param>
    /// <returns>La entidad de dominio Sale.</returns>
    public static Sale ToDomain(this VentaEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // *** INCONSISTENCIA CRÍTICA: Mapeo de VentaID (int) a SaleID (Guid) no resuelto ***
        // Por ahora, se generará un nuevo Guid. En un escenario real, necesitaríamos
        // una columna Guid en VentaEntity o una estrategia de conversión definida.
        var sale = new Sale(Guid.NewGuid(), entity.Folio)
        {
            // Si VentaEntity tuviera una columna Guid para SaleID, se usaría aquí:
            // SaleID = entity.BusinessKeyGuid,
            SaleDate = entity.FechaVenta,
            Status = (SaleStatus)entity.Estado // Mapeo explícito del enum
        };

        // Mapeo profundo de detalles de venta
        // NOTA: Para una implementación completa y AOT-friendly de ToDomain para SaleDetail,
        // necesitaríamos un mecanismo para obtener el 'Product' completo desde la capa de dominio
        // dado un 'ProductoID' (int). Esto generalmente se haría a través de un ProductRepository.
        // Aquí se usa un placeholder para 'Product'.
        // if (entity.DetalleVentas != null) // Asumiendo que VentaEntity podría tener una colección de DetalleVentaEntity
        // {
        //     foreach (var detailEntity in entity.DetalleVentas)
        //     {
        //         // Asume un método para obtener Product por ID de persistencia
        //         // var product = ProductRepository.GetProductByPersistenceId(detailEntity.ProductoID);
        //         // if (product != null)
        //         // {
        //         //     sale.AddSaleDetail(detailEntity.ToDomain(product));
        //         // }
        //     }
        // }

        return sale;
    }

    /// <summary>
    /// Transforma una entidad de dominio Sale a una entidad de persistencia VentaEntity.
    /// NOTA CRÍTICA: El mapeo de SaleID (Guid) a VentaID (int) es un marcador de posición.
    /// Sin una estrategia clara para esta conversión, se usa 0 como valor predeterminado.
    /// </summary>
    /// <param name="domain">La entidad de dominio Sale a transformar.</param>
    /// <returns>La entidad de persistencia VentaEntity.</returns>
    public static VentaEntity ToEntity(this Sale domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        // *** INCONSISTENCIA CRÍTICA: Mapeo de SaleID (Guid) a VentaID (int) no resuelto ***
        // Se usa 0 como marcador de posición. Necesitaríamos una estrategia para asignar un int
        // a partir de un Guid, o que la base de datos maneje el ID autoincremental y lo devuelva.
        var entity = new VentaEntity(0, domain.Folio) // 0 como ID de marcador de posición
        {
            // Si VentaEntity tuviera una columna Guid para SaleID, se usaría aquí:
            // BusinessKeyGuid = domain.SaleID,
            FechaVenta = domain.SaleDate,
            Estado = (int)domain.Status // Mapeo explícito del enum a int
        };

        // Mapeo profundo de detalles de venta
        // NOTA: Para una implementación completa de ToEntity para SaleDetail,
        // necesitaríamos un mapeador de Product a ProductoEntity que resuelva la inconsistencia de ID (Guid vs int).
        // Sin un ProductoEntity.ProductoID válido (int) que provenga de un Product.ProductID (Guid),
        // este mapeo de detalles sería incompleto.
        // foreach (var detailDomain in domain.SaleDetails)
        // {
        //     // Asume un método para obtener ProductoEntity.ProductoID dado Product.ProductID (Guid)
        //     // var productoEntityId = ProductMapper.ToEntity(detailDomain.Product).ProductoID;
        //     // entity.AddDetalleVenta(detailDomain.ToEntity(productoEntityId));
        // }

        return entity;
    }

    /// <summary>
    /// Transforma una entidad de persistencia DetalleVentaEntity a una entidad de dominio SaleDetail.
    /// NOTA CRÍTICA: La creación de SaleDetail requiere un objeto Product. Aquí se usa un marcador de posición.
    /// Necesitamos un mecanismo para obtener el objeto Product completo dado su ProductoID (int) de persistencia.
    /// </summary>
    /// <param name="entity">La DetalleVentaEntity a transformar.</param>
    /// <param name="product">El objeto Product de dominio asociado. Debe ser obtenido externamente.</param>
    /// <returns>La entidad de dominio SaleDetail.</returns>
    public static SaleDetail ToDomain(this DetalleVentaEntity entity, Product product)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(product); // Product es mandatorio para SaleDetail

        return new SaleDetail(product, entity.Cantidad)
        {
            UnitPrice = entity.PrecioUnitario
            // TotalDetail es una propiedad calculada en el dominio, no se mapea directamente.
        };
    }

    /// <summary>
    /// Transforma una entidad de dominio SaleDetail a una entidad de persistencia DetalleVentaEntity.
    /// NOTA CRÍTICA: El mapeo de Product.ProductID (Guid) a DetalleVentaEntity.ProductoID (int) es un marcador de posición.
    /// Se usa 0 como valor predeterminado. Se necesita una estrategia clara para esta conversión.
    /// </summary>
    /// <param name="domain">La entidad de dominio SaleDetail a transformar.</param>
    /// <param name="ventaId">El ID de la venta a la que pertenece este detalle.</param>
    /// <returns>La entidad de persistencia DetalleVentaEntity.</returns>
    public static DetalleVentaEntity ToEntity(this SaleDetail domain, int ventaId)
    {
        ArgumentNullException.ThrowIfNull(domain);

        // *** INCONSISTENCIA CRÍTICA: Mapeo de Product.ProductID (Guid) a DetalleVentaEntity.ProductoID (int) no resuelto ***
        // Se usa 0 como marcador de posición. Necesitaríamos una estrategia para asignar un int
        // a partir de un Guid, o que la base de datos maneje el ID autoincremental.
        // Idealmente, el 'ProductoID' (int) provendría de un 'ProductoEntity' ya mapeado.
        return new DetalleVentaEntity(
            0, // DetalleVentaID (int) - autogenerado o asignado por DB
            ventaId,
            0, // ProductoID (int) - marcador de posición
            domain.Quantity,
            domain.UnitPrice
        );
    }
}
```

## 3. Ejemplo de Uso dentro de un Repositorio (Snippet corto)

```csharp
using System.Data.SqlClient; // O el driver específico que se use
using Dapper; // Asumiendo Dapper para el acceso a datos
using utmMarker.Core.Entities;
using utmMarker.Infrastructure.Models.Data;
using utmMarker.Infrastructure.Mappers;

namespace utmMarker.Infrastructure.Repositories;

// Nota: Esta es una clase de repositorio de ejemplo para ilustrar el uso del mapeador.
// Necesitaría ser completada con la lógica de acceso a datos real.
public class SaleRepository // Asume una interfaz ISaleRepository
{
    private readonly string _connectionString;

    public SaleRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Sale?> GetSaleByIdAsync(int ventaId)
    {
        const string sql = "SELECT * FROM dbo.Ventas WHERE VentaID = @VentaID;";
        // También necesitaríamos cargar los detalles de la venta y los productos asociados.
        // Esto podría hacerse con múltiples consultas Dapper o una consulta JOIN si el esquema lo permite.

        await using var connection = new SqlConnection(_connectionString);
        var ventaEntity = await connection.QueryFirstOrDefaultAsync<VentaEntity>(sql, new { VentaID = ventaId });

        if (ventaEntity == null)
        {
            return null;
        }

        // Mapear la entidad de persistencia a la entidad de dominio.
        // NOTA CRÍTICA: Para un mapeo completo de Sale.ToDomain, necesitaríamos
        // la colección de DetalleVentaEntity y un ProductMapper funcional.
        var saleDomain = ventaEntity.ToDomain();
        
        // Aquí se recuperaría y mapearía la colección de detalles,
        // asumiendo que también existe un ProductRepository para obtener los objetos Product.
        // var detalleEntities = await connection.QueryAsync<DetalleVentaEntity>("SELECT * FROM dbo.DetalleVentas WHERE VentaID = @VentaID", new { VentaID = ventaId });
        // foreach (var detEnt in detalleEntities)
        // {
        //     var product = await _productRepository.GetProductByPersistenceIdAsync(detEnt.ProductoID);
        //     if (product != null)
        //     {
        //         saleDomain.AddSaleDetail(detEnt.ToDomain(product));
        //     }
        // }

        return saleDomain;
    }

    public async Task AddSaleAsync(Sale sale)
    {
        ArgumentNullException.ThrowIfNull(sale);

        // Mapear la entidad de dominio a la entidad de persistencia.
        // NOTA CRÍTICA: La VentaEntity resultante tendrá 0 como VentaID debido a la inconsistencia.
        // Un ID generado por la base de datos debería ser capturado tras la inserción.
        var ventaEntity = sale.ToEntity();

        const string sql = @"
            INSERT INTO dbo.Ventas (Folio, FechaVenta, Estado)
            VALUES (@Folio, @FechaVenta, @Estado);
            SELECT SCOPE_IDENTITY();"; // Para obtener el ID generado

        await using var connection = new SqlConnection(_connectionString);
        var newVentaId = await connection.ExecuteScalarAsync<int>(sql, ventaEntity);
        
        // Aquí se guardarían los detalles de la venta, asociándolos al newVentaId.
        // foreach (var detail in sale.SaleDetails)
        // {
        //     var detalleEntity = detail.ToEntity(newVentaId);
        //     // Lógica de inserción para detalleEntity
        // }
    }
}
```

## 4. Nota de Arquitectura: Beneficios de usar C# 14 Extension Members en este escenario

Los "Extension Members" (o métodos de extensión) de C# 14 son una característica poderosa que permite añadir nuevos métodos a tipos existentes sin modificar su código fuente original. En el contexto del diseño de Mappers para Clean Architecture y Native AOT, ofrecen varios beneficios clave:

1.  **Ergonomía de Código y Legibilidad Mejorada**: Permiten que el mapeo se "sienta" como parte del objeto que se está transformando (por ejemplo, `entity.ToDomain()` o `domain.ToEntity()`), lo que mejora enormemente la legibilidad y la fluidez del código. El desarrollador puede entender inmediatamente que se está realizando una conversión desde o hacia la entidad.

2.  **Encapsulación de Lógica de Mapeo**: Toda la lógica de transformación para un par de entidades específico (`Sale`/`VentaEntity` o `SaleDetail`/`DetalleVentaEntity`) se puede agrupar en una única clase estática (`SaleMapper`), manteniendo el código organizado y el mapeo desacoplado de las propias entidades.

3.  **Compatibilidad Total con Native AOT**: Los métodos de extensión son resueltos en tiempo de compilación y se traducen directamente en llamadas a métodos estáticos. Esto significa que no implican el uso de reflexión, generación de código dinámico o comportamientos que puedan interferir con el "trimming" agresivo que realiza el compilador de Native AOT en .NET 10. Son, por naturaleza, 100% compatibles con AOT, lo cual es fundamental para lograr binarios pequeños y de alto rendimiento.

4.  **Reducción de Boilerplate**: Evitan la necesidad de inyectar servicios de mapeo en cada repositorio o servicio de aplicación que necesite realizar una conversión. Simplemente se incluye el `namespace` del Mapper, y los métodos de extensión están disponibles globalmente para los tipos extendidos.

5.  **Facilidad de Prueba**: Al ser métodos estáticos puros (sin estado interno), son fáciles de probar de forma aislada, lo que contribuye a la robustez del sistema.

6.  **Extensibilidad y Escalabilidad**: A medida que el proyecto crece y se añaden nuevas entidades o variaciones de mapeo, se pueden añadir nuevos métodos de extensión sin tocar el código existente, promoviendo el principio de "Open/Closed".

En resumen, el uso de Extension Members en C# 14 para los mappers de "utmMarker" contribuye a una arquitectura limpia, de alto rendimiento y fácil de mantener, al tiempo que garantiza la plena compatibilidad con las optimizaciones de Native AOT de .NET 10.
