// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ColaboradorViewModel(IColaboradorService service, ILogradouroService logradouroService) : BaseViewModel, IQueryAttributable
{
    [ObservableProperty] private ColaboradorDto colaborador = Novo();
    [ObservableProperty] private string confirmarSenha = string.Empty;
    [ObservableProperty] private bool editando;
    public IEnumerable<AppColaboradorTipo> Tipos => Enum.GetValues<AppColaboradorTipo>();
    public IEnumerable<AppColaboradorVinculo> Vinculos => Enum.GetValues<AppColaboradorVinculo>();

    private static ColaboradorDto Novo() => new()
    {
        Nome = "", Cpf = "", DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)), Telefone = "", Email = "", Senha = "", Numero = "", Complemento = "",
        DataAdmissao = DateOnly.FromDateTime(DateTime.Today), Tipo = AppColaboradorTipo.Atendente, Vinculo = AppColaboradorVinculo.CLT,
        Endereco = new LogradouroDto { Cep = "", Nome = "", Bairro = "", Cidade = "", Estado = "", Pais = "Brasil" }
    };

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var id) && int.TryParse(id?.ToString(), out var valor))
        {
            var item = await service.ObterPorIdAsync(valor);
            if (item is not null) { Colaborador = item; Colaborador.Senha = ""; ConfirmarSenha = ""; Editando = true; Title = "Editar Colaborador"; }
        }
        else { Colaborador = Novo(); ConfirmarSenha = ""; Editando = false; Title = "Novo Colaborador"; }
    }

    [RelayCommand]
    private async Task BuscarCepAsync()
    {
        var cep = ApenasDigitos(Colaborador.Endereco?.Cep);
        if (cep.Length != 8) { await Shell.Current.DisplayAlertAsync("CEP", "Informe os 8 dígitos do CEP.", "OK"); return; }
        var endereco = await logradouroService.ObterPorCepAsync(cep);
        if (endereco is null) { await Shell.Current.DisplayAlertAsync("CEP não encontrado", "Cadastre o logradouro antes de cadastrar a pessoa.", "OK"); return; }
        Colaborador.Endereco = endereco;
        OnPropertyChanged(nameof(Colaborador));
    }

    [RelayCommand]
    private async Task SelecionarFotoAsync()
    {
        try
        {
            var escolha = await Shell.Current.DisplayActionSheetAsync("Foto do colaborador", "Cancelar", "Remover foto", "Galeria", "Câmera");
            if (escolha == "Remover foto") { Colaborador.Foto = null; OnPropertyChanged(nameof(Colaborador)); return; }
            FileResult? arquivo = escolha == "Galeria" ? await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Escolha uma foto", FileTypes = FilePickerFileType.Images }) : escolha == "Câmera" ? await MediaPicker.Default.CapturePhotoAsync() : null;
            if (arquivo is null) return;
            await using var stream = await arquivo.OpenReadAsync(); using var memoria = new MemoryStream(); await stream.CopyToAsync(memoria);
            Colaborador.Foto = new ArquivoDto { Conteudo = memoria.ToArray() }; OnPropertyChanged(nameof(Colaborador));
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Foto", $"Não foi possível obter a foto: {ex.Message}", "OK"); }
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        try
        {
            Colaborador.Cpf = ApenasDigitos(Colaborador.Cpf); Colaborador.Telefone = ApenasDigitos(Colaborador.Telefone);
            if (string.IsNullOrWhiteSpace(Colaborador.Nome) || Colaborador.Cpf.Length != 11 || Colaborador.Telefone.Length < 10 || Colaborador.Endereco is null || Colaborador.Endereco.Id == 0 || string.IsNullOrWhiteSpace(Colaborador.Numero))
                throw new InvalidOperationException("Preencha nome, CPF, telefone, CEP cadastrado e número do endereço.");
            if (!Editando && string.IsNullOrWhiteSpace(Colaborador.Senha)) throw new InvalidOperationException("Informe uma senha para o novo colaborador.");
            if (!string.IsNullOrWhiteSpace(Colaborador.Senha) && Colaborador.Senha != ConfirmarSenha) throw new InvalidOperationException("A confirmação da senha não confere.");
            if (Editando) await service.AtualizarAsync(Colaborador); else await service.AdicionarAsync(Colaborador);
            await Shell.Current.DisplayAlertAsync("Sucesso", "Colaborador salvo com sucesso.", "OK");
            await Shell.Current.GoToAsync("//colaboradores");
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", ex.Message, "OK"); }
    }

    [RelayCommand] private Task CancelarAsync() => Shell.Current.GoToAsync("//colaboradores");
    private static string ApenasDigitos(string? texto) => new((texto ?? string.Empty).Where(char.IsDigit).ToArray());
}
