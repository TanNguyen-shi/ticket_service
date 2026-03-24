namespace Ticketing.Infrastructure.Helpers.Interfaces;

public interface IPasswordHelper
{
    string HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string hashedPassword);
    bool IsHashedPassword(string value);
}

