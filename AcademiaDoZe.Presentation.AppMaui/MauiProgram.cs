// Kaio Fernandes Branco
using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Presentation.AppMaui.Configuration;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Views;
namespace AcademiaDoZe.Presentation.AppMaui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts => { });
        var config = ConfigurationHelper.CriarConfiguracaoAtual();
        if (config.DatabaseType == DatabaseType.Sqlite)
            DbInitializer.InicializarAsync(config.ConnectionString, config.DatabaseType).GetAwaiter().GetResult();
        builder.Services.AddSingleton(config).AddApplicationServices();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<DashboardViewModel>().AddTransient<LogradouroListViewModel>().AddTransient<LogradouroViewModel>();
        builder.Services.AddTransient<DashboardPage>().AddTransient<LogradouroListPage>().AddTransient<LogradouroPage>().AddTransient<ConfigPage>();
        return builder.Build();
    }
}
