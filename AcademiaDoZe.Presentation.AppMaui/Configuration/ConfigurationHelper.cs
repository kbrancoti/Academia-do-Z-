// Kaio Fernandes Branco
using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Presentation.AppMaui.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static RepositoryConfig CriarConfiguracaoAtual()
    {
        var (connectionString, databaseType) = ObterConfiguracaoAtual();
        var configuracao = new RepositoryConfig
        {
            ConnectionString = connectionString,
            DatabaseType = databaseType
        };

        WeakReferenceMessenger.Default.Register<RepositoryConfig, BancoPreferencesUpdatedMessage>(configuracao, (destinatario, _) =>
        {
            var (novaConnectionString, novoDatabaseType) = ObterConfiguracaoAtual();
            destinatario.ConnectionString = novaConnectionString;
            destinatario.DatabaseType = novoDatabaseType;
        });

        return configuracao;
    }

    public static (string ConnectionString, DatabaseType DatabaseType) ObterConfiguracaoAtual()
    {
        var tipo = Preferences.Get("DatabaseType", DatabaseType.Sqlite.ToString());
        if (!Enum.TryParse<DatabaseType>(tipo, out var databaseType))
            databaseType = DatabaseType.Sqlite;

        if (databaseType == DatabaseType.Sqlite)
        {
            var caminhoPadrao = Path.Combine(FileSystem.AppDataDirectory, "academiadoze.db");
            var caminho = Preferences.Get("SqliteCaminho", caminhoPadrao);
            return ($"Data Source={caminho}", databaseType);
        }

        var prefixo = databaseType == DatabaseType.SqlServer ? "SqlServer" : "MySql";
        var servidorPadrao = databaseType == DatabaseType.SqlServer ? "localhost" : "localhost";
        var usuarioPadrao = databaseType == DatabaseType.SqlServer ? "sa" : "root";
        var servidor = Preferences.Get($"{prefixo}Servidor", servidorPadrao);
        var banco = Preferences.Get($"{prefixo}Banco", "db_academia_do_ze");
        var usuario = Preferences.Get($"{prefixo}Usuario", usuarioPadrao);
        var senha = Preferences.Get($"{prefixo}Senha", string.Empty);
        var complemento = Preferences.Get($"{prefixo}Complemento", string.Empty);

        return ($"Server={servidor};Database={banco};User Id={usuario};Password={senha};{complemento}", databaseType);
    }
}
