using System;

namespace utmMarker.Infrastructure.Models.Data;

public partial class VentaEntity(int ventaId, string folio)
{
    public int VentaID { get; set; } = ventaId; // Changed from init to set
    public string Folio { get; init; } = folio;
    public DateTime FechaVenta { get; init; } = DateTime.Now; // Se asume que la base de datos también registra la fecha de creación
    public int TotalArticulos { get; set; } // Added to match DB schema
    public decimal TotalVenta { get; set; } // Added to match DB schema
    public int Estatus { get; set; } // int para mapear el enum SaleStatus
}
