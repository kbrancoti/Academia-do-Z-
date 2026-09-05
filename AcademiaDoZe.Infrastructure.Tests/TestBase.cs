// Kaio Fernandes Branco
using AcademiaDoZe.Infrastructure.Data;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]
namespace AcademiaDoZe.Infrastructure.Tests;

public abstract class TestBase
{
    protected const DatabaseType SelectedDatabaseType = DatabaseType.Sqlite;
    protected string ConnectionString { get; }
    protected DatabaseType DatabaseType { get; }

    protected TestBase()
    {
        DatabaseType = SelectedDatabaseType;
        var databaseFile = Path.Combine(Path.GetTempPath(), "AcademiaDoZe", "db_academia_do_ze.db");
        Directory.CreateDirectory(Path.GetDirectoryName(databaseFile)!);
        ConnectionString = DatabaseType switch
        {
            DatabaseType.SqlServer => "Server=localhost;Database=db_academia_do_ze;User Id=ze;Password=abcBolinhas12345;TrustServerCertificate=True;Encrypt=True;",
            DatabaseType.MySql => "Server=localhost;Port=3307;Database=db_academia_do_ze;User Id=ze;Password=abcBolinhas12345;",
            DatabaseType.Sqlite => $"Data Source={databaseFile};Cache=Shared;",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static int _counter;
    protected static string GerarCep()
    {
        var seed = (int)(DateTime.UtcNow.Ticks % 9_000_000);
        return (80_000_000 + ((seed + Interlocked.Increment(ref _counter)) % 9_000_000)).ToString("D8");
    }

    protected static string GerarCpf()
    {
        var baseDigits = (100_000_000 + ((int)(DateTime.UtcNow.Ticks % 800_000_000) + Interlocked.Increment(ref _counter)) % 800_000_000).ToString("D9");
        var numbers = baseDigits.Select(character => character - '0').ToArray();
        var first = CalculateCheckDigit(numbers, 10);
        var second = CalculateCheckDigit(numbers.Append(first).ToArray(), 11);
        return baseDigits + first + second;
    }

    private static int CalculateCheckDigit(IReadOnlyList<int> digits, int weight)
    {
        var sum = digits.Select((digit, index) => digit * (weight - index)).Sum();
        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}
