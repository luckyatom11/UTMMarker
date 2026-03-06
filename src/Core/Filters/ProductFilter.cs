namespace utmMarker.Core.Filters;

/// <summary>
/// Representa los criterios de filtrado para la entidad Product.
/// Diseñado para ser simple y compatible con Native AOT.
/// </summary>
public sealed class ProductFilter
{
    /// <summary>
    /// Filtra productos por nombre (búsqueda parcial o exacta).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Filtra productos por SKU (búsqueda exacta).
    /// </summary>
    public string? SKU { get; init; }

    /// <summary>
    /// Filtra productos por marca (búsqueda parcial o exacta).
    /// </summary>
    public string? Brand { get; init; }

    /// <summary>
    /// Filtra productos con precio mayor o igual a este valor.
    /// </summary>
    public decimal? MinPrice { get; init; }

    /// <summary>
    /// Filtra productos con precio menor o igual a este valor.
    /// </summary>
    public decimal? MaxPrice { get; init; }

    /// <summary>
    /// Filtra productos con stock mayor o igual a este valor.
    /// </summary>
    public int? MinStock { get; init; }

    /// <summary>
    /// Filtra productos con stock menor o igual a este valor.
    /// </summary>
    public int? MaxStock { get; init; }
}
