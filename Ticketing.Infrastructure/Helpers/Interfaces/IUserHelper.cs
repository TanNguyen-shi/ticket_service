namespace Ticketing.Infrastructure.Helpers.Interfaces;

public interface IUserHelper
{
    string Username { get; }
    long? UserId { get; }
    bool IsAuthenticated { get; }
}