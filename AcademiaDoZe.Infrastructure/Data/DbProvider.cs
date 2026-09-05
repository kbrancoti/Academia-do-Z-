// Kaio Fernandes Branco
using AcademiaDoZe.Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Data;

public enum DatabaseType { SqlServer, MySql, Sqlite }

public static class DbProvider
{
    public const int DefaultCommandTimeout = 30;

    public static DbConnection CreateConnection(string connectionString, DatabaseType dbType)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InfrastructureException("CONEXAO_STRING_VAZIA", "String de conexão não pode ser vazia.");
        try
        {
            return dbType switch
            {
                DatabaseType.SqlServer => new SqlConnection(connectionString),
                DatabaseType.MySql => new MySqlConnection(connectionString),
                DatabaseType.Sqlite => new SqliteConnection(connectionString),
                _ => throw new InfrastructureException("SGDB_NAO_SUPORTADO", $"SGDB não suportado: {dbType}")
            };
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("FALHA_CONEXAO", $"Falha ao criar conexão para {dbType}.", ex);
        }
    }

    public static DbCommand CreateCommand(string commandText, DbConnection connection, CommandType commandType = CommandType.Text)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (string.IsNullOrWhiteSpace(commandText))
            throw new InfrastructureException("COMANDO_TEXTO_VAZIO", "Texto do comando não pode ser vazio.");
        try
        {
            var command = connection.CreateCommand() ?? throw new InfrastructureException("FALHA_CRIAR_COMANDO", "Falha ao criar comando.");
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.CommandTimeout = DefaultCommandTimeout;
            return command;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("FALHA_CRIAR_COMANDO", "Falha ao criar comando.", ex);
        }
    }

    public static DbParameter AddParameter(this DbCommand command, string name, object? value, DbType dbType)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(name)) throw new InfrastructureException("PARAMETRO_NOME_VAZIO", "Nome do parâmetro não pode ser vazio.");
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        parameter.DbType = dbType;
        command.Parameters.Add(parameter);
        return parameter;
    }

    public static async Task<int> ExecuteScalarIdAsync(this DbCommand command, string errorCode = "ERRO_OBTER_ID", string errorMessage = "Falha ao obter ID inserido no banco de dados.", CancellationToken cancellationToken = default)
    {
        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (result is not null and not DBNull) return Convert.ToInt32(result);
        throw new InfrastructureException(errorCode, errorMessage);
    }

    public static string GetScriptName(DatabaseType dbType) => dbType switch
    {
        DatabaseType.SqlServer => "script_sqlserver.sql", DatabaseType.MySql => "script_mysql.sql", DatabaseType.Sqlite => "script_sqlite.sql",
        _ => throw new InfrastructureException("SGDB_NAO_SUPORTADO", $"SGDB não suportado: {dbType}")
    };

    public static string FormatInsertQuery(string insertSql, DatabaseType dbType)
    {
        if (string.IsNullOrWhiteSpace(insertSql)) throw new InfrastructureException("SQL_INSERT_VAZIO", "Comando INSERT não pode ser vazio.");
        return dbType switch
        {
            DatabaseType.SqlServer => $"{insertSql}; SELECT CAST(SCOPE_IDENTITY() AS INT);",
            DatabaseType.MySql => $"{insertSql}; SELECT LAST_INSERT_ID();",
            DatabaseType.Sqlite => $"{insertSql}; SELECT last_insert_rowid();",
            _ => throw new InfrastructureException("SGDB_NAO_SUPORTADO", $"SGDB não suportado: {dbType}")
        };
    }

    public static string GetCurrentDateFunction(DatabaseType dbType) => dbType switch { DatabaseType.SqlServer => "GETDATE()", DatabaseType.MySql => "CURRENT_DATE()", DatabaseType.Sqlite => "DATE('now')", _ => throw new InfrastructureException("SGDB_NAO_SUPORTADO", $"SGDB não suportado: {dbType}") };
    public static string GetDateAddDaysExpression(string dateExpr, string daysParam, DatabaseType dbType) => dbType switch { DatabaseType.SqlServer => $"DATEADD(day, {daysParam}, {dateExpr})", DatabaseType.MySql => $"DATE_ADD({dateExpr}, INTERVAL {daysParam} DAY)", DatabaseType.Sqlite => $"DATE({dateExpr}, '+' || {daysParam} || ' days')", _ => throw new InfrastructureException("SGDB_NAO_SUPORTADO", $"SGDB não suportado: {dbType}") };
    public static string GetDateHourExpression(string dateColumn, DatabaseType dbType) => dbType switch { DatabaseType.SqlServer => $"DATEPART(HOUR, {dateColumn})", DatabaseType.MySql => $"HOUR({dateColumn})", DatabaseType.Sqlite => $"CAST(strftime('%H', {dateColumn}) AS INTEGER)", _ => throw new InfrastructureException("SGDB_NAO_SUPORTADO", $"SGDB não suportado: {dbType}") };
    public static string GetDateMonthExpression(string dateColumn, DatabaseType dbType) => dbType switch { DatabaseType.SqlServer => $"MONTH({dateColumn})", DatabaseType.MySql => $"MONTH({dateColumn})", DatabaseType.Sqlite => $"CAST(strftime('%m', {dateColumn}) AS INTEGER)", _ => throw new InfrastructureException("SGDB_NAO_SUPORTADO", $"SGDB não suportado: {dbType}") };
    public static string GetDateDayExpression(string dateColumn, DatabaseType dbType) => dbType switch { DatabaseType.SqlServer => $"DAY({dateColumn})", DatabaseType.MySql => $"DAY({dateColumn})", DatabaseType.Sqlite => $"CAST(strftime('%d', {dateColumn}) AS INTEGER)", _ => throw new InfrastructureException("SGDB_NAO_SUPORTADO", $"SGDB não suportado: {dbType}") };
}
