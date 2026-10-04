// Kaio Fernandes Branco
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Presentation.AppMaui.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();
        CarregarTema();
        CarregarBanco();
    }

    private void CarregarTema() => TemaPicker.SelectedIndex = Preferences.Get("Tema", "system") switch
    {
        "light" => 0,
        "dark" => 1,
        _ => 2
    };

    private void CarregarBanco()
    {
        BancoPicker.SelectedIndex = Preferences.Get("DatabaseType", DatabaseType.Sqlite.ToString()) switch
        {
            nameof(DatabaseType.SqlServer) => 1,
            nameof(DatabaseType.MySql) => 2,
            _ => 0
        };
        AtualizarCamposBanco();
    }

    private void OnBancoChanged(object? sender, EventArgs e) => AtualizarCamposBanco();

    private void AtualizarCamposBanco()
    {
        var tipo = ObterTipoSelecionado();
        SqliteContainer.IsVisible = tipo == DatabaseType.Sqlite;
        ServidorContainer.IsVisible = tipo != DatabaseType.Sqlite;

        if (tipo == DatabaseType.Sqlite)
        {
            SqliteCaminhoEntry.Text = Preferences.Get("SqliteCaminho", Path.Combine(FileSystem.AppDataDirectory, "academiadoze.db"));
            return;
        }

        var prefixo = tipo == DatabaseType.SqlServer ? "SqlServer" : "MySql";
        ServidorEntry.Text = Preferences.Get($"{prefixo}Servidor", "localhost");
        BancoEntry.Text = Preferences.Get($"{prefixo}Banco", "db_academia_do_ze");
        UsuarioEntry.Text = Preferences.Get($"{prefixo}Usuario", tipo == DatabaseType.SqlServer ? "sa" : "root");
        SenhaEntry.Text = Preferences.Get($"{prefixo}Senha", string.Empty);
        ComplementoEntry.Text = Preferences.Get($"{prefixo}Complemento", string.Empty);
    }

    private async void OnSalvarTemaClicked(object sender, EventArgs e)
    {
        var tema = TemaPicker.SelectedIndex switch { 0 => "light", 1 => "dark", _ => "system" };
        Preferences.Set("Tema", tema);
        WeakReferenceMessenger.Default.Send(new TemaPreferencesUpdatedMessage(tema));
        await DisplayAlertAsync("Tema atualizado", "A preferência de tema foi salva.", "OK");
    }

    private async void OnSalvarBancoClicked(object sender, EventArgs e)
    {
        var tipo = ObterTipoSelecionado();
        if (tipo == DatabaseType.Sqlite)
        {
            if (string.IsNullOrWhiteSpace(SqliteCaminhoEntry.Text))
            {
                await DisplayAlertAsync("Validação", "Informe o caminho do arquivo SQLite.", "OK");
                return;
            }
            Preferences.Set("SqliteCaminho", SqliteCaminhoEntry.Text.Trim());
        }
        else
        {
            if (string.IsNullOrWhiteSpace(ServidorEntry.Text) || string.IsNullOrWhiteSpace(BancoEntry.Text) || string.IsNullOrWhiteSpace(UsuarioEntry.Text))
            {
                await DisplayAlertAsync("Validação", "Preencha servidor, banco e usuário.", "OK");
                return;
            }
            var prefixo = tipo == DatabaseType.SqlServer ? "SqlServer" : "MySql";
            Preferences.Set($"{prefixo}Servidor", ServidorEntry.Text.Trim());
            Preferences.Set($"{prefixo}Banco", BancoEntry.Text.Trim());
            Preferences.Set($"{prefixo}Usuario", UsuarioEntry.Text.Trim());
            Preferences.Set($"{prefixo}Senha", SenhaEntry.Text ?? string.Empty);
            Preferences.Set($"{prefixo}Complemento", ComplementoEntry.Text?.Trim() ?? string.Empty);
        }

        Preferences.Set("DatabaseType", tipo.ToString());
        WeakReferenceMessenger.Default.Send(new BancoPreferencesUpdatedMessage(tipo.ToString()));
        await DisplayAlertAsync("Banco atualizado", "As credenciais foram salvas e aplicadas à configuração da aplicação.", "OK");
    }

    private DatabaseType ObterTipoSelecionado() => BancoPicker.SelectedIndex switch { 1 => DatabaseType.SqlServer, 2 => DatabaseType.MySql, _ => DatabaseType.Sqlite };

    private async void OnCancelarClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//dashboard");

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
