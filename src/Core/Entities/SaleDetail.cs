namespace utmMarker.Core.Entities;

public sealed class SaleDetail(
    int detalleID,
    int ventaID,
    int productoID,
    decimal unitPrice,
    int quantity,
    decimal totalDetail)
{
    public int DetalleID { get; set; } = detalleID;
    public int VentaID { get; set; } = ventaID;
    public int ProductoID { get; set; } = productoID;
    public decimal UnitPrice { get; init; } = unitPrice > 0 ? unitPrice : throw new ArgumentOutOfRangeException(nameof(unitPrice), "El precio unitario debe ser mayor que cero.");
    public int Quantity { get; init; } = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor que cero.");

    // Propiedad calculada con cuerpo de expresión
    public decimal TotalDetail { get; init; } = totalDetail;

    // Add a parameterless constructor for ORM/Dapper mapping if needed (can be private)
    private SaleDetail() : this(0, 0, 0, 0, 0, 0) { }
}
