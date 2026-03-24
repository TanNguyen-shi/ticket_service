using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Ticketing.Infrastructure.Configurations;

public class DapperContext(IConfiguration configuration)
{
    public NpgsqlConnection CreateConnection()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

        return new NpgsqlConnection(connectionString);
    }
}