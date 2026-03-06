namespace utmMarker.Infrastructure.Models.Data;

public partial class DetalleVentaEntity(int detalleVentaId, int ventaId, int productoId, int cantidad, decimal precioUnitario)
{
    public int DetalleVentaID { get; set; } = detalleVentaId;
    public int VentaID { get; set; } = ventaId;
    public int ProductoID { get; set; } = productoId;

    private int _cantidad = cantidad; // Initialized from constructor parameter
    public int Cantidad
    {
        get => _cantidad;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Cantidad), "La cantidad debe ser mayor que cero.");
            }
            _cantidad = value;
        }
    }

    private decimal _precioUnitario = precioUnitario; // Initialized from constructor parameter
    public decimal PrecioUnitario
    {
        get => _precioUnitario;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(PrecioUnitario), "El precio unitario no puede ser negativo.");
            }
            _precioUnitario = value;
        }
    }

    public decimal TotalDetalle { get; init; } = cantidad * precioUnitario;
}
