using Authentication.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Authentication.Contracts.Config;

namespace Authentication.Services.Implementations;

public class PasswordHasher :IPasswordHasher
{
    private readonly PasswordHasherOptions _options;

    public PasswordHasher(IOptions<PasswordHasherOptions> options)
    {
        _options = options.Value;
    }

    public string Hash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[_options.SaltSize];
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            _options.Iterations,
            HashAlgorithmName.SHA256,
            _options.HashSize);

        var result = new byte[_options.SaltSize + _options.HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, _options.SaltSize);
        Buffer.BlockCopy(hash, 0, result, _options.SaltSize, _options.HashSize);

        return Convert.ToBase64String(result);
    }

    public bool Verify(string password, string hashedPassword)
    {
        var bytes = Convert.FromBase64String(hashedPassword);

        var salt = new byte[_options.SaltSize];
        Buffer.BlockCopy(bytes, 0, salt, 0, _options.SaltSize);

        var hash = new byte[_options.HashSize];
        Buffer.BlockCopy(bytes, _options.SaltSize, hash, 0, _options.HashSize);

        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            _options.Iterations,
            HashAlgorithmName.SHA256,
            _options.HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
    }
}