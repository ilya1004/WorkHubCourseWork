using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using IdentityService.DAL.Abstractions.PasswordHasher;
using Konscious.Security.Cryptography;

namespace IdentityService.BLL.Services.PasswordHasher;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 32;  // 256 bit
    private const int MemoryCost = 65536;   // 64 MB
    private const int Iterations = 3;
    private const int Parallelism = 2;

    private const int MinLength = 6;
    private static readonly Regex HasUpperCase = new("[A-Z]", RegexOptions.Compiled);
    private static readonly Regex HasLowerCase = new("[a-z]", RegexOptions.Compiled);
    private static readonly Regex HasDigit = new(@"\d", RegexOptions.Compiled);
    private static readonly Regex HasSpecialChar = new(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]", RegexOptions.Compiled);
    private static readonly Regex HasWhitespace = new(@"\s", RegexOptions.Compiled);

    public string HashPassword(string password)
    {
        if (!ValidatePassword(password, out var error))
        {
            throw new ArgumentException(error, nameof(password));
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            MemorySize = MemoryCost,
            Iterations = Iterations
        };

        var hash = argon2.GetBytes(HashSize);

        return $"$argon2id$v=19$m={MemoryCost},t={Iterations},p={Parallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) || !hashedPassword.StartsWith("$argon2id$"))
        {
            return false;
        }

        var parts = hashedPassword.Split('$');
        if (parts.Length != 5)
        {
            return false;
        }

        var version = int.Parse(parts[2].Substring(2));
        if (version != 19)
        {
            return false;
        }

        var parameters = parts[3].Split(',');
        var memory = int.Parse(parameters[0].Substring(2));
        var iterations = int.Parse(parameters[1].Substring(2));
        var parallelism = int.Parse(parameters[2].Substring(2));

        var salt = Convert.FromBase64String(parts[4]);
        var expectedHash = Convert.FromBase64String(parts[5]);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = parallelism,
            MemorySize = memory,
            Iterations = iterations
        };

        var actualHash = argon2.GetBytes(expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private bool ValidatePassword(string password, out string? errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "Password cannot be empty.";
            return false;
        }

        if (password.Length < MinLength)
        {
            errorMessage = $"Password must contain more than {MinLength} symbols.";
            return false;
        }

        if (HasWhitespace.IsMatch(password))
        {
            errorMessage = "Password cannot contain whitespaces.";
            return false;
        }

        if (!HasUpperCase.IsMatch(password))
        {
            errorMessage = "Password must contain at least one upper case.";
            return false;
        }

        if (!HasLowerCase.IsMatch(password))
        {
            errorMessage = "Password must contain at least one lower case.";
            return false;
        }

        if (!HasDigit.IsMatch(password))
        {
            errorMessage = "Password must contain at least one digit.";
            return false;
        }

        if (!HasSpecialChar.IsMatch(password))
        {
            errorMessage = "Password must contain at least one special character.";
            return false;
        }

        return true;
    }
}