# Diseño de Entidades de Dominio Puras para utmMarker (Clean Architecture & Native AOT)

## 1. Definición del 'Enum' de Estatus de Venta (`SaleStatus.cs`)

Este enumerado define los posibles estados de una venta en el sistema, asegurando la coherencia y facilitando la lógica de negocio basada en el ciclo de vida de la venta.

```csharp
namespace utmMarker.Core.Enums;

public enum SaleStatus
{
    Pending,    // La venta ha sido iniciada pero no completada.
    Completed,  // La venta se ha finalizado y los productos han sido entregados/enviados.
    Cancelled,  // La venta ha sido cancelada antes de su finalización.
    Returned    // Los productos de la venta han sido devueltos.
}
```

## 2. Código Fuente Completo y Documentado para Cada Clase

### `Product.cs`

La entidad `Product` encapsula la información fundamental de un producto, aplicando validaciones de negocio en sus propiedades `Price` y `Stock` utilizando la palabra clave `field` de C# 14.

```csharp
namespace utmMarker.Core.Entities;

public sealed class Product(Guid productId, string name, string sku, string brand)
{
    /// <summary>
    /// Identificador único del producto (Primary Key).
    /// </summary>
    public Guid ProductID { get; init; } = productId;

    /// <summary>
    /// Nombre del producto.
    /// </summary>
    public string Name { get; init; } = name;

    /// <summary>
    /// Código de referencia único del producto.
    /// </summary>
    public string SKU { get; init; } = sku;

    /// <summary>
    /// Marca del producto.
    /// </summary>
    public string Brand { get; init; } = brand;

    /// <summary>
    /// Precio unitario del producto. No puede ser negativo.
    /// </summary>
    public decimal Price
    {
        get => field; // Utiliza 'field' para acceder al campo de respaldo.
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Price), "El precio no puede ser negativo.");
            }
            field = value; // Utiliza 'field' para asignar al campo de respaldo.
        }
    }

    /// <summary>
    /// Cantidad de unidades disponibles en stock. No puede ser negativo.
    /// </summary>
    public int Stock
    {
        get => field; // Utiliza 'field' para acceder al campo de respaldo.
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Stock), "El stock no puede ser negativo.");
            }
            field = value; // Utiliza 'field' para asignar al campo de respaldo.
        }
    }
}
```

### `SaleDetail.cs`

La entidad `SaleDetail` representa un ítem dentro de una venta. Mantiene una referencia al `Product` y captura el `UnitPrice` en el momento de la creación para preservar el historial de precios. Incluye una propiedad calculada `TotalDetail`.

```csharp
namespace utmMarker.Core.Entities;

public sealed class SaleDetail(Product product, int quantity)
{
    /// <summary>
    /// El producto asociado a este detalle de venta.
    /// </summary>
    public Product Product { get; init; } = product ?? throw new ArgumentNullException(nameof(product));
    
    /// <summary>
    /// Cantidad del producto vendida en este detalle.
    /// </summary>
    public int Quantity { get; init; } = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor que cero.");
    
    /// <summary>
    /// Precio unitario del producto en el momento de la venta.
    /// Se inicializa desde Product.Price pero es independiente para preservar el precio histórico.
    /// </summary>
    public decimal UnitPrice { get; init; } = product.Price;

    /// <summary>
    /// Propiedad calculada que representa el total de este detalle (UnitPrice * Quantity).
    /// </summary>
    public decimal TotalDetail => UnitPrice * Quantity;
}
```

### `Sale.cs`

La entidad `Sale` representa una venta completa, conteniendo una colección de `SaleDetail`. Gestiona el estado de la venta, la fecha de creación y proporciona propiedades calculadas para el total de ítems y el monto total de la venta.

```csharp
using utmMarker.Core.Enums;

namespace utmMarker.Core.Entities;

public sealed class Sale(Guid saleId, string folio)
{
    /// <summary>
    /// Identificador único de la venta (Primary Key).
    /// </summary>
    public Guid SaleID { get; init; } = saleId;

    /// <summary>
    /// Folio o número de referencia de la venta.
    /// </summary>
    public string Folio { get; init; } = folio;

    /// <summary>
    /// Fecha y hora en que se realizó la venta. Se inicializa automáticamente con la fecha y hora actual.
    /// </summary>
    public DateTime SaleDate { get; init; } = DateTime.Now;

    /// <summary>
    /// El estado actual de la venta.
    /// </summary>
    public SaleStatus Status { get; set; } = SaleStatus.Pending;

    private readonly List<SaleDetail> _saleDetails = [];
    /// <summary>
    /// Colección de detalles de la venta. Se expone como una lista de solo lectura.
    /// </summary>
    public IReadOnlyList<SaleDetail> SaleDetails => _saleDetails.AsReadOnly();

    /// <summary>
    /// Agrega un detalle de venta a la colección.
    /// </summary>
    /// <param name="detail">El detalle de venta a agregar.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el detalle es nulo.</exception>
    public void AddSaleDetail(SaleDetail detail)
    {
        ArgumentNullException.ThrowIfNull(detail);
        _saleDetails.Add(detail);
    }

    /// <summary>
    /// Elimina un detalle de venta de la colección.
    /// </summary>
    /// <param name="detail">El detalle de venta a eliminar.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el detalle es nulo.</exception>
    public void RemoveSaleDetail(SaleDetail detail)
    {
        ArgumentNullException.ThrowIfNull(detail);
        _saleDetails.Remove(detail);
    }

    /// <summary>
    /// Propiedad calculada que devuelve el número total de ítems en la venta.
    /// </summary>
    public int TotalItems => _saleDetails.Sum(detail => detail.Quantity);
    
    /// <summary>
    /// Propiedad calculada que devuelve el monto total de la venta, sumando los totales de cada detalle.
    /// </summary>
    public decimal TotalSale => _saleDetails.Sum(detail => detail.TotalDetail);
}
```

## 3. Breve explicación de cómo la palabra clave `field` ayudó a reducir el boilerplate en las validaciones de negocio.

La palabra clave `field` introducida en C# 14 permite acceder al campo de respaldo implícito de una propiedad autoimplementada directamente desde sus accesores (`get` o `set`). Antes de C# 14, si se necesitaba añadir lógica a un accesor de una propiedad autoimplementada (como validaciones o notificaciones), se requería convertirla en una propiedad con un campo de respaldo explícito (`private int _price; public int Price { get => _price; set => _price = value; }`). Esto añadía `boilerplate` al código, haciendo las clases más verborreicas.

Con `field`, este proceso se simplifica significativamente. Como se demostró en la entidad `Product`, las validaciones para `Price` y `Stock` se pueden implementar directamente en los accesores `set` utilizando `field` sin la necesidad de declarar manualmente un campo privado.

**Ejemplo comparativo:**

**Antes (C# 13 y anteriores):**
```csharp
private decimal _price;
public decimal Price
{
    get { return _price; }
    set
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Price), "El precio no puede ser negativo.");
        }
        _price = value;
    }
}
```

**Ahora (C# 14 con `field`):**
```csharp
public decimal Price
{
    get => field; // Accede al campo de respaldo implícito
    set
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Price), "El precio no puede ser negativo.");
        }
        field = value; // Asigna al campo de respaldo implícito
    }
}
```

Este cambio mejora la concisión y la legibilidad del código al tiempo que mantiene la capacidad de encapsular la lógica de negocio directamente dentro de la propiedad, lo cual es crucial para mantener la pureza del dominio en un enfoque DDD y asegurar la compatibilidad con Native AOT al evitar la reflexión o la generación de código dinámico.
