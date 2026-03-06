using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices; // Added for EnumeratorCancellation
using Microsoft.Data.SqlClient; // Explicitly use Microsoft.Data.SqlClient
using utmMarker.Core.Entities;
using utmMarker.Core.Filters;
using utmMarker.Core.Repositories;
using utmMarker.Infrastructure.Data;
using utmMarker.Infrastructure.Mappers;
using utmMarker.Infrastructure.Models.Data;

public sealed class ProductRepositoryImpl(IDbConnectionFactory connectionFactory) : IProductRepository
{
    // SQL Queries
    private const string GetAllSql = "SELECT ProductoID, Nombre, SKU, Marca, Precio, Stock FROM Producto;";
    private const string GetByIdSql = "SELECT ProductoID, Nombre, SKU, Marca, Precio, Stock FROM Producto WHERE ProductoID = @ProductoID;";
    private const string AddSql = "INSERT INTO Producto (Nombre, SKU, Marca, Precio, Stock) VALUES (@Nombre, @SKU, @Marca, @Precio, @Stock); SELECT SCOPE_IDENTITY();";
    private const string UpdateSql = "UPDATE Producto SET Nombre = @Nombre, SKU = @SKU, Marca = @Marca, Precio = @Precio, Stock = @Stock WHERE ProductoID = @ProductoID;";
    private const string UpdateStockSql = "UPDATE Producto SET Stock = @Stock WHERE ProductoID = @ProductoID;";
    private const string DeleteSql = "DELETE FROM Producto WHERE ProductoID = @ProductoID;";

    public async IAsyncEnumerable<Product> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var command = new Microsoft.Data.SqlClient.SqlCommand(GetAllSql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var entity = MapToProductoEntity(reader);
            yield return ProductMapper.ToDomain(entity);
        }
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var command = new Microsoft.Data.SqlClient.SqlCommand(GetByIdSql, connection);
        command.Parameters.AddWithValue("@ProductoID", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            var entity = MapToProductoEntity(reader);
            return ProductMapper.ToDomain(entity);
        }

        return null;
    }

    public async IAsyncEnumerable<Product> FindAsync(ProductFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        var (sql, parameters) = BuildFilterQuery(filter);
        await using var command = new Microsoft.Data.SqlClient.SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters.ToArray());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var entity = MapToProductoEntity(reader);
            yield return ProductMapper.ToDomain(entity);
        }
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        var entity = ProductMapper.ToEntity(product); // Convert domain to entity
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var command = new Microsoft.Data.SqlClient.SqlCommand(AddSql, connection);
        // Exclude ProductoID from parameters for INSERT as it's an IDENTITY column
        command.Parameters.AddWithValue("@Nombre", entity.Nombre);
        command.Parameters.AddWithValue("@SKU", entity.SKU);
        command.Parameters.AddWithValue("@Marca", entity.Marca);
        command.Parameters.AddWithValue("@Precio", entity.Precio);
        command.Parameters.AddWithValue("@Stock", entity.Stock);
        
        var newId = await command.ExecuteScalarAsync(cancellationToken);
        if (newId is decimal decimalId) // SCOPE_IDENTITY() returns decimal
        {
            // Create a new Product instance with the generated ID, retaining other properties
            return new Product((int)decimalId, product.Name, product.SKU, product.Brand, product.Price, product.Stock);
        }
        else
        {
            throw new InvalidOperationException("Failed to retrieve new product ID after insertion.");
        }
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var entity = ProductMapper.ToEntity(product); // Convert domain to entity
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var command = new Microsoft.Data.SqlClient.SqlCommand(UpdateSql, connection);
        AddProductParameters(command, entity); // This now includes ProductID for WHERE clause

        await command.ExecuteNonQueryAsync(cancellationToken);
        return product; // Return the updated product
    }

    public async Task UpdateStockAsync(int productId, int newStock, CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var command = new Microsoft.Data.SqlClient.SqlCommand(UpdateStockSql, connection);
        command.Parameters.AddWithValue("@ProductoID", productId);
        command.Parameters.AddWithValue("@Stock", newStock);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = (Microsoft.Data.SqlClient.SqlConnection)(await connectionFactory.CreateConnectionAsync());
        await connection.OpenAsync(cancellationToken);

        await using var command = new Microsoft.Data.SqlClient.SqlCommand(DeleteSql, connection);
        command.Parameters.AddWithValue("@ProductoID", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    // Helper method to map SqlDataReader to ProductoEntity
    private static ProductoEntity MapToProductoEntity(Microsoft.Data.SqlClient.SqlDataReader reader)
    {
        return new ProductoEntity(
            productoId: reader.GetInt32(reader.GetOrdinal("ProductoID")),
            sku: reader.GetString(reader.GetOrdinal("SKU"))
        )
        {
            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
            Marca = reader.GetString(reader.GetOrdinal("Marca")),
            Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
            Stock = reader.GetInt32(reader.GetOrdinal("Stock"))
        };
    }

    // Helper method to add parameters for Product (ProductoEntity)
    private static void AddProductParameters(Microsoft.Data.SqlClient.SqlCommand command, ProductoEntity entity)
    {
        command.Parameters.AddWithValue("@ProductoID", entity.ProductoID);
        command.Parameters.AddWithValue("@Nombre", entity.Nombre);
        command.Parameters.AddWithValue("@SKU", entity.SKU);
        command.Parameters.AddWithValue("@Marca", entity.Marca);
        command.Parameters.AddWithValue("@Precio", entity.Precio);
        command.Parameters.AddWithValue("@Stock", entity.Stock);
    }

    // Helper method to build dynamic SQL query for filtering
    private static (string sql, List<Microsoft.Data.SqlClient.SqlParameter> parameters) BuildFilterQuery(ProductFilter filter)
    {
        var sqlBuilder = new System.Text.StringBuilder("SELECT ProductoID, Nombre, SKU, Marca, Precio, Stock FROM Producto WHERE 1=1");
        var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            sqlBuilder.Append(" AND Nombre LIKE @Name");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Name", $"%{filter.Name}%"));
        }
        if (!string.IsNullOrWhiteSpace(filter.SKU))
        {
            sqlBuilder.Append(" AND SKU = @SKU");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SKU", filter.SKU));
        }
        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            sqlBuilder.Append(" AND Marca LIKE @Marca");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Marca", $"%{filter.Brand}%"));
        }
        if (filter.MinPrice.HasValue)
        {
            sqlBuilder.Append(" AND Precio >= @MinPrice");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@MinPrice", filter.MinPrice.Value));
        }
        if (filter.MaxPrice.HasValue)
        {
            sqlBuilder.Append(" AND Precio <= @MaxPrice");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@MaxPrice", filter.MaxPrice.Value));
        }
        if (filter.MinStock.HasValue)
        {
            sqlBuilder.Append(" AND Stock >= @MinStock");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@MinStock", filter.MinStock.Value));
        }
        if (filter.MaxStock.HasValue)
        {
            sqlBuilder.Append(" AND Stock <= @MaxStock");
            parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@MaxStock", filter.MaxStock.Value));
        }

        return (sqlBuilder.ToString(), parameters);
    }
}
