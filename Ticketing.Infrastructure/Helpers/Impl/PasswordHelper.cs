using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace Ticketing.Infrastructure.Helpers.Impl;

public class PasswordHelper : IPasswordHelper
{
    private const string Prefix = "pbkdf2";
    private const int IterationCount = 100000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public string HashPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password is required", nameof(plainPassword));

        if (IsHashedPassword(plainPassword))
            return plainPassword;

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = KeyDerivation.Pbkdf2(
            password: plainPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: IterationCount,
            numBytesRequested: KeySize);

        return string.Join("$", Prefix, IterationCount, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        if (!TryParseHashedPassword(hashedPassword, out var iterations, out var salt, out var expectedHash))
            return false;

        var actualHash = KeyDerivation.Pbkdf2(
            password: plainPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: iterations,
            numBytesRequested: expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    public bool IsHashedPassword(string value)
    {
        return TryParseHashedPassword(value, out _, out _, out _);
    }

    private static bool TryParseHashedPassword(
        string value,
        out int iterations,
        out byte[] salt,
        out byte[] hash)
    {
        iterations = 0;
        salt = Array.Empty<byte>();
        hash = Array.Empty<byte>();

        var parts = value.Split('$');
        if (parts.Length != 4)
            return false;

        if (!string.Equals(parts[0], Prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!int.TryParse(parts[1], out iterations) || iterations <= 0)
            return false;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            hash = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        return salt.Length > 0 && hash.Length > 0;
    }
}

