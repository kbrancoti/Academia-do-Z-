// Kaio Fernandes Branco
using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.DependencyInjection;
namespace AcademiaDoZe.Presentation.AppMaui;
public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        InitializeComponent();
        Items.Add(CriarItem("Dashboard", "dashboard", () => services.GetRequiredService<DashboardPage>()));
        Items.Add(CriarItem("Logradouros", "logradouros", () => services.GetRequiredService<LogradouroListPage>()));
        Routing.RegisterRoute("logradouro", typeof(LogradouroPage));
    }
    private static FlyoutItem CriarItem(string titulo, string rota, Func<Page> pagina) => new() { Title = titulo, Route = rota, Items = { new ShellContent { ContentTemplate = new DataTemplate(pagina) } } };
}
