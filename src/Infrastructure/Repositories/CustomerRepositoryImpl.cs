using System.Data;
using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Infrastructure.Data;
using Microsoft.Data.SqlClient; // Explicitly using Microsoft.Data.SqlClient

namespace utmMarker.Infrastructure.Repositories;

public class CustomerRepositoryImpl(IDbConnectionFactory connectionFactory) : ICustomerRepository
{
    public async Task<Customer?> GetByEmailAsync(string email)
    {
        Customer? customer = null;
        using (var connection = await connectionFactory.CreateConnectionAsync())
        {
            var sqlConnection = (SqlConnection)connection; // Cast to SqlConnection
            using (var command = sqlConnection.CreateCommand()) // Use sqlConnection to create command
            {
                command.CommandText = "SELECT Id, FullName, Email, IsActive FROM Customers WHERE Email = @Email";
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });

                await sqlConnection.OpenAsync(); // Use sqlConnection for OpenAsync
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        };
                    }
                }
            }
        }
        return customer;
    }

    public async Task AddAsync(Customer customer)
    {
        using (var connection = await connectionFactory.CreateConnectionAsync())
        {
            var sqlConnection = (SqlConnection)connection; // Cast to SqlConnection
            using (var command = sqlConnection.CreateCommand()) // Use sqlConnection to create command
            {
                command.CommandText = "INSERT INTO Customers (FullName, Email, IsActive) VALUES (@FullName, @Email, @IsActive)";
                command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = customer.FullName });
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = customer.Email });
                command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = customer.IsActive });

                await sqlConnection.OpenAsync(); // Use sqlConnection for OpenAsync
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customers = new List<Customer>();
        using (var connection = await connectionFactory.CreateConnectionAsync())
        {
            var sqlConnection = (SqlConnection)connection;
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = "SELECT Id, FullName, Email, IsActive FROM Customers";

                await sqlConnection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        customers.Add(new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        });
                    }
                }
            }
        }
        return customers;
    }
}
