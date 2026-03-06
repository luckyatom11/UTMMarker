namespace utmMarker.Infrastructure.Data;

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private string _connectionString = default!; // Private backing field

    public string ConnectionString
    {
        get => _connectionString;
        init
        {
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));
            // You can add more complex validation logic here for the connection string format
            if (!value.Contains("data source=") || !value.Contains("initial catalog="))
            {
                throw new ArgumentException("Invalid connection string format. Missing data source or initial catalog.", nameof(value));
            }
            _connectionString = value; // Assign to the explicit backing field
        }
    }

    public SqlConnectionFactory(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("The connection string 'DefaultConnection' cannot be null or empty.", nameof(configuration));
        }
        ConnectionString = connectionString; // Assignment triggers the 'init' accessor validation
    }

    public Task<IDbConnection> CreateConnectionAsync()
    {
        return Task.FromResult<IDbConnection>(new Microsoft.Data.SqlClient.SqlConnection(ConnectionString));
    }
}
