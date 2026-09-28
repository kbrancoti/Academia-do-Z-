// Kaio Fernandes Branco
using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Views;
namespace AcademiaDoZe.Presentation.AppMaui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts => { });
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "academiadoze.db");
        var config = new RepositoryConfig { ConnectionString = $"Data Source={dbPath}", DatabaseType = DatabaseType.Sqlite };
        DbInitializer.InicializarAsync(config.ConnectionString, config.DatabaseType).GetAwaiter().GetResult();
        builder.Services.AddSingleton(config).AddApplicationServices();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<DashboardViewModel>().AddTransient<LogradouroListViewModel>().AddTransient<LogradouroViewModel>();
        builder.Services.AddTransient<DashboardPage>().AddTransient<LogradouroListPage>().AddTransient<LogradouroPage>();
        return builder.Build();
    }
}
