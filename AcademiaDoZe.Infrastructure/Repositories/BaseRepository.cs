// Kaio Fernandes Branco
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public abstract class BaseRepository : IDisposable, IAsyncDisposable
{
    protected readonly string ConnectionString;
    protected readonly DatabaseType DatabaseType;
    private DbConnection? _connection;
    private bool _disposed;

    protected BaseRepository(string connectionString, DatabaseType databaseType)
    {
        ConnectionString = string.IsNullOrWhiteSpace(connectionString)
            ? throw new InfrastructureException("STRING_CONEXAO_NULA", "String de conexão não pode ser nula ou vazia.")
            : connectionString;
        DatabaseType = databaseType;
    }

    protected virtual async Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        try
        {
            await DbInitializer.InicializarAsync(ConnectionString, DatabaseType, cancellationToken);
            if (_connection is null) _connection = DbProvider.CreateConnection(ConnectionString, DatabaseType);
            if (_connection.State is ConnectionState.Closed or ConnectionState.Broken)
            {
                if (_connection.State == ConnectionState.Broken) await _connection.CloseAsync();
                await _connection.OpenAsync(cancellationToken);
            }
            return _connection;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("FALHA_ABRIR_CONEXAO", "Falha ao abrir conexão com o banco de dados.", ex);
        }
    }

    protected async Task<DbCommand> CreateCommandAsync(string commandText, CancellationToken cancellationToken = default) =>
        DbProvider.CreateCommand(commandText, await GetOpenConnectionAsync(cancellationToken));

    protected string FormatInsertQuery(string insertSql) => DbProvider.FormatInsertQuery(insertSql, DatabaseType);

    public void Dispose()
    {
        if (!_disposed) { _connection?.Dispose(); _connection = null; _disposed = true; }
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed && _connection is not null) await _connection.DisposeAsync();
        _connection = null; _disposed = true;
        GC.SuppressFinalize(this);
    }
}
