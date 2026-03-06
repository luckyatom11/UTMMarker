namespace utmMarker.Infrastructure.Data;

using System.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}
