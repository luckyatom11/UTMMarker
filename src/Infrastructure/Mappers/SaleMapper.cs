using utmMarker.Core.Entities;
using utmMarker.Core.Enums;
using utmMarker.Infrastructure.Models.Data;
using System.Linq;
using System.Collections.Generic;

namespace utmMarker.Infrastructure.Mappers;

public static class SaleMapper
{
    /// <summary>
    /// Transforma una entidad de persistencia VentaEntity a una entidad de dominio Sale.
    /// </summary>
    /// <param name="entity">La VentaEntity a transformar.</param>
    /// <returns>La entidad de dominio Sale.</returns>
    public static Sale ToDomain(this VentaEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new Sale(
            entity.VentaID,
            entity.Folio,
            entity.FechaVenta,
            entity.TotalArticulos,
            entity.TotalVenta,
            (SaleStatus)entity.Estatus
        );
    }

    /// <summary>
    /// Transforma una entidad de dominio Sale a una entidad de persistencia VentaEntity.
    /// </summary>
    /// <param name="domain">La entidad de dominio Sale a transformar.</param>
    /// <returns>La entidad de persistencia VentaEntity.</returns>
    public static VentaEntity ToEntity(this Sale domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        var entity = new VentaEntity(domain.SaleID, domain.Folio)
        {
            FechaVenta = domain.SaleDate,
            Estatus = (int)domain.Status,
            TotalArticulos = domain.TotalItems, // Map new property
            TotalVenta = domain.TotalSale // Map new property
        };

        return entity;
    }

    /// <summary>
    /// Transforma una entidad de persistencia DetalleVentaEntity a una entidad de dominio SaleDetail.
    /// </summary>
    /// <param name="entity">La DetalleVentaEntity a transformar.</param>
    /// <returns>La entidad de dominio SaleDetail.</returns>
    public static SaleDetail ToDomain(this DetalleVentaEntity entity) // Removed Product product parameter
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new SaleDetail(
            entity.DetalleVentaID,
            entity.VentaID,
            entity.ProductoID,
            entity.PrecioUnitario, // Corrected order
            entity.Cantidad,      // Corrected order
            entity.PrecioUnitario * entity.Cantidad // Calculated TotalDetail
        );
    }

    /// <summary>
    /// Transforma una entidad de dominio SaleDetail a una entidad de persistencia DetalleVentaEntity.
    /// </summary>
    /// <param name="domain">La entidad de dominio SaleDetail a transformar.</param>
    /// <returns>La entidad de persistencia DetalleVentaEntity.</returns>
    public static DetalleVentaEntity ToEntity(this SaleDetail domain) // Removed detalleVentaId, ventaId parameters
    {
        ArgumentNullException.ThrowIfNull(domain);

        return new DetalleVentaEntity(
            domain.DetalleID,
            domain.VentaID,
            domain.ProductoID,
            domain.Quantity,
            domain.UnitPrice
        );
    }
}
