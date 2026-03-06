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
