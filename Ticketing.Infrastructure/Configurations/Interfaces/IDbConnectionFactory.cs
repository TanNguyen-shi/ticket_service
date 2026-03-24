using System.Data;

namespace Ticketing.Application.Interfaces.Configurations;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}