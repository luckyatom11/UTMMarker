# Diseño de Entidades de Base de Datos para utmMarker (Dapper AOT)

Se ha generado la entidad `ProductoEntity` basándose en el esquema DDL proporcionado. Sin embargo, para `DetalleVentaEntity` y `VentaEntity`, el esquema DDL de SQL Server 2022 no fue incluido en las instrucciones. Para replicar con exactitud el esquema de la base de datos, necesito que se proporcionen los DDLs para las tablas `dbo.DetalleVenta` y `dbo.Venta`.

A continuación, se presenta el código generado para `ProductoEntity` y una nota técnica sobre el mapeo manual en entornos Native AOT.

## 1. Código Fuente de Entidades Generadas

### `ProductoEntity.cs`

Esta clase representa la entidad de base de datos para la tabla `dbo.Producto`, adhiriéndose a las convenciones de C# 14 y Native AOT. Incluye validaciones básicas para replicar las restricciones `CHECK` de SQL.

```csharp
namespace utmMarker.Infrastructure.Models.Data;

// public partial class para permitir extensiones estáticas de mapeo
public partial class ProductoEntity(int productoId, string sku)
{
    // ProductoID (INT PK) - Primary Constructor para campos obligatorios
    public int ProductoID { get; init; } = productoId;

    // Nombre (NVARCHAR 100)
    public string Nombre { get; init; } = null!; // null! para indicar que no es nulo en SQL, pero no se valida aquí.

    // SKU (VARCHAR 20 UNIQUE) - Primary Constructor para campos obligatorios
    public string SKU { get; init; } = sku;

    // Marca (NVARCHAR 50)
    public string Marca { get; init; } = null!; // null! para indicar que no es nulo en SQL, pero no se valida aquí.

    private decimal _precio;
    // Precio (DECIMAL 19,4) - Validaciones de restricciones CHECK de SQL usando 'field'
    public decimal Precio
    {
        get => field;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Precio), "El precio no puede ser negativo.");
            }
            field = value;
        }
    }

    private int _stock;
    // Stock (INT) - Validaciones de restricciones CHECK de SQL usando 'field'
    public int Stock
    {
        get => field;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Stock), "El stock no puede ser negativo.");
            }
            field = value;
        }
    }
}
```

## 2. Nota técnica: Mapeo manual vs. ORMs dinámicos en .NET 10 (Native AOT)

En .NET 10 y especialmente cuando se apunta a Native AOT, el mapeo manual de datos (por ejemplo, utilizando `SqlDataReader` directamente o con micro-ORMs como Dapper en modo AOT-friendly) ofrece ventajas significativas sobre los ORMs dinámicos (como Entity Framework Core con algunas de sus características por defecto).

### ¿Por qué el mapeo manual es superior para Native AOT?

1.  **Compatibilidad con Trimming Agresivo**: Las herramientas de AOT de .NET realizan un "trimming" (recorte) agresivo para eliminar código y metadatos no utilizados. Los ORMs que dependen fuertemente de la reflexión en tiempo de ejecución (para descubrir tipos, propiedades, construir consultas dinámicas, etc.) pueden confundir al trimmer, forzándolo a mantener una gran cantidad de código que de otra manera sería eliminado. Esto aumenta el tamaño del binario final y la huella de memoria. El mapeo manual o Dapper con generadores de código fuente (source generators) específicos para AOT evita este problema al hacer que el patrón de acceso a datos sea estático y conocido en tiempo de compilación.

2.  **Rendimiento en Tiempo de Ejecución**: La reflexión tiene una sobrecarga en tiempo de ejecución. Al evitarla, el mapeo manual elimina esta sobrecarga, resultando en un acceso a datos más rápido y eficiente. En escenarios de alto rendimiento o donde cada milisegundo cuenta (como microservicios), esta diferencia puede ser crucial.

3.  **Tamaño del Binario Reducido**: Como se mencionó, al evitar la reflexión y el código dinámico, el compilador AOT puede producir binarios considerablemente más pequeños. Esto es ideal para aplicaciones CLI ligeras, contenedores Docker pequeños y funciones serverless, donde el tamaño del artefacto de despliegue y la velocidad de inicio son primordiales.

4.  **Control Explicito**: El mapeo manual otorga un control explícito sobre cómo se transfieren los datos entre la base de datos y los objetos C#. Esto puede ser beneficioso para optimizaciones muy específicas o cuando se trabaja con modelos de datos complejos.

### Consideraciones con Dapper en Native AOT

Dapper, siendo un micro-ORM, es inherentemente más ligero que un ORM completo. Sin embargo, su uso tradicional con mapeo automático aún puede implicar algo de reflexión. Para garantizar la máxima compatibilidad con Native AOT y evitar sorpresas con el trimming, se recomienda:
*   Utilizar las API de Dapper que permiten un mapeo más explícito (por ejemplo, `Query<T>(sql, new { id = ... })` donde `T` es un POCO) y asegurar que el mapeo de columnas a propiedades sea directo.
*   Explorar el uso de Source Generators de Dapper (si están disponibles y son maduros para .NET 10) o realizar el mapeo de `SqlDataReader` a mano cuando se requiere el máximo control y certeza AOT. Las entidades de base de datos (`ProductoEntity`) generadas en este proyecto están diseñadas como POCOs simples para facilitar este tipo de mapeo eficiente.

Este enfoque asegura que el proyecto `utmMarker` aproveche al máximo las optimizaciones de .NET 10, produciendo una aplicación CLI extremadamente ligera, de alto rendimiento y compatible con Native AOT.

---
**NOTA:** Para generar `DetalleVentaEntity` y `VentaEntity`, por favor, proporcione el esquema DDL exacto de las tablas `dbo.DetalleVenta` y `dbo.Venta`.
