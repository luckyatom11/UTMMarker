# Diseño de Mapeador de Productos para utmMarker (AOT-Compatible)

## Inconsistencia Crítica Detectada: Mapeo de `ProductID`

Se ha detectado una inconsistencia crítica en el tipo de la propiedad identificadora `ProductID` entre la entidad de dominio (`Product.cs`) y la entidad de persistencia (`ProductoEntity.cs`):

*   **Dominio (`Product.cs`):** `public Guid ProductID { get; init; }`
*   **Persistencia (`ProductoEntity.cs`):** `public int ProductoID { get; init; }`

Dado que `ProductID` es la clave primaria y un identificador único en ambas capas, esta disparidad de tipos (`Guid` vs `int`) impide la creación de un mapeador bidireccional correcto y fiable sin una estrategia de conversión explícita. El esquema SQL proporcionado (`ProductoID (INT PK)`) para `dbo.Producto` indica que la base de datos espera un `int`.

Para proceder con la implementación del `ProductMapper.cs`, es **fundamental** aclarar cómo se debe manejar esta conversión. Las opciones comunes incluyen:

1.  **Añadir un campo `Guid` a `ProductoEntity`**: Mantener el `int ProductoID` como clave subrogada de la base de datos y añadir una propiedad `Guid ProductGuid` a `ProductoEntity` que mapee al `ProductID` del dominio. Esta es a menudo la solución preferida para desacoplar el dominio de la base de datos.
2.  **Modificar el dominio**: Cambiar `ProductID` en `Product.cs` a `int`, alineándose con la capa de persistencia (menos ideal para DDD si el `Guid` es un identificador de negocio natural).
3.  **Estrategia de conversión `Guid` a `int`**: Esto es muy poco común y a menudo no recomendado para claves primarias debido a la posible pérdida de unicidad y complejidad.

**Se requiere su guía para resolver esta inconsistencia antes de poder generar el `ProductMapper.cs` de manera completa y correcta.**

---

Mientras se espera la aclaración, a continuación se presenta la estructura del proyecto y una explicación preliminar de las propiedades identificadas.

### Árbol de Directorios Actualizado

```
C:\Programacion\utmMarker
├───Program.cs
├───utmMarker.csproj
├───utmMarker.sln
├───bin
├───db
├───docs
│   ├───ARCHITECTURE.md
│   └───DOMAIN_ENTITIES.md
│   └───INFRASTRUCTURE_MODELS.md
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
    │   └───Models
    │       └───Data
    │           └───ProductoEntity.cs
```

### Propiedades Identificadas para Mapeo (Excluyendo `ProductID` temporalmente)

*   **`Product.cs` (Dominio)**
    *   `ProductID`: `Guid` (Inconsistencia crítica)
    *   `Name`: `string`
    *   `SKU`: `string`
    *   `Brand`: `string`
    *   `Price`: `decimal`
    *   `Stock`: `int`

*   **`ProductoEntity.cs` (Persistencia)**
    *   `ProductoID`: `int` (Inconsistencia crítica)
    *   `Nombre`: `string`
    *   `SKU`: `string`
    *   `Marca`: `string`
    *   `Precio`: `decimal`
    *   `Stock`: `int`

**Discrepancias de Nombres (Se resolverán en el mapeador):**
*   `Product.Name` <-> `ProductoEntity.Nombre`
*   `Product.Brand` <-> `ProductoEntity.Marca`
*   `Product.Price` <-> `ProductoEntity.Precio`

Una vez que se resuelva la forma de manejar la propiedad `ProductID`, se generará el código completo de `ProductMapper.cs` con métodos de extensión, comentarios XML, y un ejemplo de uso, junto con la justificación de las decisiones de diseño.

---
**POR FAVOR, INDIQUE CÓMO DEBO PROCEDER CON EL MAPEO DE `ProductID` (Guid vs int).**
