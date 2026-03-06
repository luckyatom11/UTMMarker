namespace utmMarker.Infrastructure.Models.Data;

// public partial class para permitir extensiones estáticas de mapeo
public partial class ProductoEntity(int productoId, string sku) // Changed Guid to int
{
    // ProductoID (INT PK) - Primary Constructor para campos obligatorios
    public int ProductoID { get; init; } = productoId; // Changed Guid to int

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
        get => _precio; // Corrected to use explicit backing field
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Precio), "El precio no puede ser negativo.");
            }
            _precio = value; // Corrected to use explicit backing field
        }
    }

    private int _stock;
    // Stock (INT) - Validaciones de restricciones CHECK de SQL usando 'field'
    public int Stock
    {
        get => _stock; // Corrected to use explicit backing field
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Stock), "El stock no puede ser negativo.");
            }
            _stock = value; // Corrected to use explicit backing field
        }
    }
}
