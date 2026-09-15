// Kaio Fernandes Branco
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace AcademiaDoZe.Application.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16, HashSize = 32, Iterations = 3, MemorySizeKb = 64 * 1024;
    public static string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var parallelism = Math.Max(1, Environment.ProcessorCount);
        var hash = Criar(password, salt, Iterations, MemorySizeKb, parallelism).GetBytes(HashSize);
        return $"ARGON2ID:{Iterations}:{MemorySizeKb}:{parallelism}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }
    public static bool Verify(string password, string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split(':');
            if (string.IsNullOrWhiteSpace(password) || parts.Length != 6 || parts[0] != "ARGON2ID") return false;
            var salt = Convert.FromBase64String(parts[4]); var expected = Convert.FromBase64String(parts[5]);
            var actual = Criar(password, salt, int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3])).GetBytes(expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch { return false; }
    }
    private static Argon2id Criar(string senha, byte[] salt, int iteracoes, int memoria, int paralelismo) => new(Encoding.UTF8.GetBytes(senha)) { Salt = salt, Iterations = Math.Max(1, iteracoes), MemorySize = Math.Max(1024, memoria), DegreeOfParallelism = Math.Max(1, paralelismo) };
}
