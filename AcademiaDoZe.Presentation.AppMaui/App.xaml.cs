// Kaio Fernandes Branco
using AcademiaDoZe.Presentation.AppMaui.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly AppShell _shell;

    public App(AppShell shell)
    {
        InitializeComponent();
        _shell = shell;
        AplicarTema();
        WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(this, (_, _) => AplicarTema());
    }

    protected override Window CreateWindow(IActivationState? activationState) => new(_shell);

    private void AplicarTema()
    {
        UserAppTheme = Preferences.Get("Tema", "system") switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
