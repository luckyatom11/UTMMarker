namespace utmMarker.Infrastructure.Repositories;

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices; // Added for EnumeratorCancellation
using Microsoft.Data.SqlClient; // Explicitly use Microsoft.Data.SqlClient
using utmMarker.Core.Entities;
using utmMarker.Core.Enums;
using utmMarker.Core.Filters;
using utmMarker.Core.Repositories;
using utmMarker.Infrastructure.Data;
using utmMarker.Infrastructure.Mappers;
using utmMarker.Infrastructure.Models.Data;

public sealed class SaleRepositoryImpl(IDbConnectionFactory connectionFactory) : ISaleRepository // Removed IProductRepository productRepository
{
    // SQL Queries
    private const string GetAllSalesSql = "SELECT VentaID, Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus FROM Venta;";
    private const string GetSaleByIdSql = "SELECT VentaID, Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus FROM Venta WHERE VentaID = @VentaID;";
    private const string GetSaleDetailsBySaleIdSql = "SELECT DetalleID, VentaID, ProductoID, Cantidad, PrecioUnitario, TotalDetalle FROM DetalleVenta WHERE VentaID = @VentaID;";
    private const string AddSaleSql = "INSERT INTO Venta (Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus) VALUES (@Folio, @FechaVenta, @TotalArticulos, @TotalVenta, @Estatus); SELECT SCOPE_IDENTITY();";
    private const string AddSaleDetailSql = "INSERT INTO DetalleVenta (VentaID, ProductoID, Cantidad, PrecioUnitario, TotalDetalle) VALUES (@VentaID, @ProductoID, @Cantidad, @PrecioUnitario, @TotalDetalle); SELECT SCOPE_IDENTITY();";
    private const string UpdateSaleSql = "UPDATE Venta SET Folio = @Folio, FechaVenta = @FechaVenta, TotalArticulos = @TotalArticulos, TotalVenta = @TotalVenta, Estatus = @Estatus WHERE VentaID = @VentaID;";
    private const string DeleteSaleDetailsBySaleIdSql = "DELETE FROM DetalleVenta WHERE VentaID = @VentaID;";
    private const string DeleteSaleSql = "DELETE FROM Venta WHERE VentaID = @VentaID;";

    public async IAsyncEnumerable<Sale> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var command = new Microsoft.Data.SqlClient.SqlCommand(GetAllSalesSql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var sales = new List<Sale>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var ventaEntity = MapToVentaEntity(reader);
            sales.Add(SaleMapper.ToDomain(ventaEntity));
        }
        await reader.CloseAsync();

        foreach (var sale in sales)
        {
            await using var detailCommand = new Microsoft.Data.SqlClient.SqlCommand(GetSaleDetailsBySaleIdSql, connection);
            detailCommand.Parameters.AddWithValue("@VentaID", sale.SaleID);
            await using var detailReader = await detailCommand.ExecuteReaderAsync(cancellationToken);
            while (await detailReader.ReadAsync(cancellationToken))
            {
                var detalleVentaEntity = MapToDetalleVentaEntity(detailReader);
                sale.AddSaleDetail(SaleMapper.ToDomain(detalleVentaEntity)); // Mapped directly
            }
            yield return sale;
        }
    }

    public async Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        // Fetch sale header
        await using var saleCommand = new Microsoft.Data.SqlClient.SqlCommand(GetSaleByIdSql, connection);
        saleCommand.Parameters.AddWithValue("@VentaID", id);
        await using var saleReader = await saleCommand.ExecuteReaderAsync(cancellationToken);

        Sale? sale = null;
        if (await saleReader.ReadAsync(cancellationToken))
        {
            var ventaEntity = MapToVentaEntity(saleReader);
            sale = SaleMapper.ToDomain(ventaEntity);
        }
        saleReader.Close();

        if (sale != null)
        {
            // Fetch sale details
            await using var detailCommand = new Microsoft.Data.SqlClient.SqlCommand(GetSaleDetailsBySaleIdSql, connection);
            detailCommand.Parameters.AddWithValue("@VentaID", sale.SaleID);
            await using var detailReader = await detailCommand.ExecuteReaderAsync(cancellationToken);
            while (await detailReader.ReadAsync(cancellationToken))
            {
                var detalleVentaEntity = MapToDetalleVentaEntity(detailReader);
                sale.AddSaleDetail(SaleMapper.ToDomain(detalleVentaEntity)); // Mapped directly
            }
        }
        return sale;
    }

    public async IAsyncEnumerable<Sale> FindAsync(SaleFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        var (sql, parameters) = BuildFilterQuery(filter);
        await using var command = new Microsoft.Data.SqlClient.SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters.ToArray());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var sales = new List<Sale>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var ventaEntity = MapToVentaEntity(reader);
            sales.Add(SaleMapper.ToDomain(ventaEntity));
        }
        reader.Close();

        foreach (var sale in sales)
        {
            await using var detailCommand = new Microsoft.Data.SqlClient.SqlCommand(GetSaleDetailsBySaleIdSql, connection);
            detailCommand.Parameters.AddWithValue("@VentaID", sale.SaleID);
            await using var detailReader = await detailCommand.ExecuteReaderAsync(cancellationToken);
            while (await detailReader.ReadAsync(cancellationToken))
            {
                var detalleVentaEntity = MapToDetalleVentaEntity(detailReader);
                sale.AddSaleDetail(SaleMapper.ToDomain(detalleVentaEntity)); // Mapped directly
            }
            yield return sale;
        }
    }

    public async Task<Sale> AddAsync(Sale sale, IEnumerable<SaleDetail> saleDetails, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sale);
        ArgumentNullException.ThrowIfNull(saleDetails);

        var ventaEntity = SaleMapper.ToEntity(sale);

        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            // Insert Sale Header
            await using var addSaleCommand = new Microsoft.Data.SqlClient.SqlCommand(AddSaleSql, connection, (Microsoft.Data.SqlClient.SqlTransaction)transaction);
            AddVentaEntityParameters(addSaleCommand, ventaEntity);
            var newSaleId = await addSaleCommand.ExecuteScalarAsync(cancellationToken);
            if (newSaleId is decimal decimalSaleId)
            {
                ventaEntity.VentaID = (int)decimalSaleId;
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve new sale ID after insertion.");
            }

            // Insert Sale Details using the provided saleDetails parameter
            foreach (var detail in saleDetails)
            {
                var detalleVentaEntity = SaleMapper.ToEntity(detail); // Convert domain to entity
                detalleVentaEntity.VentaID = ventaEntity.VentaID; // Assign new SaleID to detail

                await using var addDetailCommand = new Microsoft.Data.SqlClient.SqlCommand(AddSaleDetailSql, connection, (Microsoft.Data.SqlClient.SqlTransaction)transaction);
                AddDetalleVentaEntityParameters(addDetailCommand, detalleVentaEntity);
                var newDetailId = await addDetailCommand.ExecuteScalarAsync(cancellationToken);
                if (newDetailId is decimal decimalDetailId)
                {
                    // Update the SaleDetail with generated DetalleID
                    detail.DetalleID = (int)decimalDetailId;
                    detail.VentaID = ventaEntity.VentaID; // Ensure VentaID is set in domain detail
                }
                else
                {
                    throw new InvalidOperationException("Failed to retrieve new sale detail ID after insertion.");
                }
            }
            await transaction.CommitAsync(cancellationToken);
            
            // Reconstruct Sale object with generated IDs
            var addedSale = new Sale(
                ventaEntity.VentaID,
                ventaEntity.Folio,
                ventaEntity.FechaVenta,
                ventaEntity.TotalArticulos,
                ventaEntity.TotalVenta,
                (SaleStatus)ventaEntity.Estatus
            );

            foreach (var detail in saleDetails)
            {
                addedSale.AddSaleDetail(detail); // Use the AddSaleDetail method
            }
            return addedSale;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sale);

        var ventaEntity = SaleMapper.ToEntity(sale);

        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            // Update Sale Header
            await using var updateSaleCommand = new Microsoft.Data.SqlClient.SqlCommand(UpdateSaleSql, connection, (Microsoft.Data.SqlClient.SqlTransaction)transaction);
            AddVentaEntityParameters(updateSaleCommand, ventaEntity);
            await updateSaleCommand.ExecuteNonQueryAsync(cancellationToken);

            // Delete existing Sale Details
            await using var deleteDetailsCommand = new Microsoft.Data.SqlClient.SqlCommand(DeleteSaleDetailsBySaleIdSql, connection, (Microsoft.Data.SqlClient.SqlTransaction)transaction);
            deleteDetailsCommand.Parameters.AddWithValue("@VentaID", sale.SaleID);
            await deleteDetailsCommand.ExecuteNonQueryAsync(cancellationToken);

            // Insert new Sale Details
            foreach (var detail in sale.SaleDetails)
            {
                var detalleVentaEntity = SaleMapper.ToEntity(detail); // Convert domain to entity
                detalleVentaEntity.VentaID = sale.SaleID; // Assign SaleID to detail

                await using var addDetailCommand = new Microsoft.Data.SqlClient.SqlCommand(AddSaleDetailSql, connection, (Microsoft.Data.SqlClient.SqlTransaction)transaction);
                AddDetalleVentaEntityParameters(addDetailCommand, detalleVentaEntity);
                var newDetailId = await addDetailCommand.ExecuteScalarAsync(cancellationToken);
                if (newDetailId is decimal decimalDetailId)
                {
                    detail.DetalleID = (int)decimalDetailId; // Update the SaleDetail with generated DetalleID
                }
                else
                {
                    throw new InvalidOperationException("Failed to retrieve new sale detail ID after insertion during update.");
                }
            }
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            // Delete Sale Details first due to foreign key constraints
            await using var deleteDetailsCommand = new Microsoft.Data.SqlClient.SqlCommand(DeleteSaleDetailsBySaleIdSql, connection, (Microsoft.Data.SqlClient.SqlTransaction)transaction);
            deleteDetailsCommand.Parameters.AddWithValue("@VentaID", id);
            await deleteDetailsCommand.ExecuteNonQueryAsync(cancellationToken);

            // Delete Sale Header
            await using var deleteSaleCommand = new Microsoft.Data.SqlClient.SqlCommand(DeleteSaleSql, connection, (Microsoft.Data.SqlClient.SqlTransaction)transaction);
            deleteSaleCommand.Parameters.AddWithValue("@VentaID", id);
            await deleteSaleCommand.ExecuteNonQueryAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    // Helper method to map SqlDataReader to VentaEntity
    private static VentaEntity MapToVentaEntity(Microsoft.Data.SqlClient.SqlDataReader reader)
    {
        return new VentaEntity(
            ventaId: reader.GetInt32(reader.GetOrdinal("VentaID")),
            folio: reader.GetString(reader.GetOrdinal("Folio"))
        )
        {
            FechaVenta = reader.GetDateTime(reader.GetOrdinal("FechaVenta")),
            TotalArticulos = reader.GetInt32(reader.GetOrdinal("TotalArticulos")),
            TotalVenta = reader.GetDecimal(reader.GetOrdinal("TotalVenta")),
            Estatus = (int)reader.GetByte(reader.GetOrdinal("Estatus"))
        };
    }

    // Helper method to map SqlDataReader to DetalleVentaEntity
    private static DetalleVentaEntity MapToDetalleVentaEntity(Microsoft.Data.SqlClient.SqlDataReader reader)
    {
        return new DetalleVentaEntity(
            detalleVentaId: reader.GetInt32(reader.GetOrdinal("DetalleID")),
            ventaId: reader.GetInt32(reader.GetOrdinal("VentaID")),
            productoId: reader.GetInt32(reader.GetOrdinal("ProductoID")), // Corrected parameter name
            cantidad: reader.GetInt32(reader.GetOrdinal("Cantidad")),
            precioUnitario: reader.GetDecimal(reader.GetOrdinal("PrecioUnitario"))
        )
        {
            TotalDetalle = reader.GetDecimal(reader.GetOrdinal("TotalDetalle"))
        };
    }

    // Helper method to add parameters for VentaEntity
    private static void AddVentaEntityParameters(Microsoft.Data.SqlClient.SqlCommand command, VentaEntity entity)
    {
        command.Parameters.AddWithValue("@VentaID", entity.VentaID);
        command.Parameters.AddWithValue("@Folio", entity.Folio);
        command.Parameters.AddWithValue("@FechaVenta", entity.FechaVenta);
        command.Parameters.AddWithValue("@TotalArticulos", entity.TotalArticulos);
        command.Parameters.AddWithValue("@TotalVenta", entity.TotalVenta);
        command.Parameters.AddWithValue("@Estatus", entity.Estatus);
    }

    // Helper method to add parameters for DetalleVentaEntity
    private static void AddDetalleVentaEntityParameters(Microsoft.Data.SqlClient.SqlCommand command, DetalleVentaEntity entity)
    {
        // DetalleID is IDENTITY, so it's not passed for insert
        command.Parameters.AddWithValue("@VentaID", entity.VentaID);
        command.Parameters.AddWithValue("@ProductoID", entity.ProductoID);
        command.Parameters.AddWithValue("@Cantidad", entity.Cantidad);
        command.Parameters.AddWithValue("@PrecioUnitario", entity.PrecioUnitario);
        command.Parameters.AddWithValue("@TotalDetalle", entity.TotalDetalle);
    }

    // Helper method to build dynamic SQL query for filtering
    private static (string sql, List<Microsoft.Data.SqlClient.SqlParameter> parameters) BuildFilterQuery(SaleFilter filter)
    {
        var sqlBuilder = new System.Text.StringBuilder("SELECT VentaID, Folio, FechaVenta, TotalArticulos, TotalVenta, Estatus FROM Venta WHERE 1=1");
        var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();

        if (filter.MinSaleDate.HasValue)
        {
            sqlBuilder.Append(" AND FechaVenta >= @MinSaleDate");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@MinSaleDate", filter.MinSaleDate.Value));
        }
        if (filter.MaxSaleDate.HasValue)
        {
            sqlBuilder.Append(" AND FechaVenta <= @MaxSaleDate");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@MaxSaleDate", filter.MaxSaleDate.Value));
        }
        if (!string.IsNullOrWhiteSpace(filter.Folio))
        {
            sqlBuilder.Append(" AND Folio LIKE @Folio");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Folio", $"%{filter.Folio}%"));
        }
        if (filter.Status.HasValue)
        {
            sqlBuilder.Append(" AND Estatus = @Estatus");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Estatus", (int)filter.Status.Value));
        }

        return (sqlBuilder.ToString(), parameters);
    }
}
