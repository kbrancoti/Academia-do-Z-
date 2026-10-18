// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class AlunoViewModel(IAlunoService service, ILogradouroService logradouroService) : BaseViewModel, IQueryAttributable
{
    [ObservableProperty] private AlunoDto aluno = Novo();
    [ObservableProperty] private string confirmarSenha = string.Empty;
    [ObservableProperty] private bool editando;

    private static AlunoDto Novo() => new()
    {
        Nome = "", Cpf = "", DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)), Telefone = "", Email = "", Senha = "", Numero = "", Complemento = "",
        Endereco = new LogradouroDto { Cep = "", Nome = "", Bairro = "", Cidade = "", Estado = "", Pais = "Brasil" }
    };

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var id) && int.TryParse(id?.ToString(), out var valor))
        {
            var item = await service.ObterPorIdAsync(valor);
            if (item is not null) { Aluno = item; Aluno.Senha = ""; ConfirmarSenha = ""; Editando = true; Title = "Editar Aluno"; }
        }
        else { Aluno = Novo(); ConfirmarSenha = ""; Editando = false; Title = "Novo Aluno"; }
    }

    [RelayCommand]
    private async Task BuscarCepAsync()
    {
        var cep = ApenasDigitos(Aluno.Endereco?.Cep);
        if (cep.Length != 8) { await Shell.Current.DisplayAlertAsync("CEP", "Informe os 8 dígitos do CEP.", "OK"); return; }
        var endereco = await logradouroService.ObterPorCepAsync(cep);
        if (endereco is null) { await Shell.Current.DisplayAlertAsync("CEP não encontrado", "Cadastre o logradouro antes de cadastrar a pessoa.", "OK"); return; }
        Aluno.Endereco = endereco;
        OnPropertyChanged(nameof(Aluno));
    }

    [RelayCommand]
    private async Task SelecionarFotoAsync()
    {
        try
        {
            var escolha = await Shell.Current.DisplayActionSheetAsync("Foto do aluno", "Cancelar", "Remover foto", "Galeria", "Câmera");
            if (escolha == "Remover foto") { Aluno.Foto = null; OnPropertyChanged(nameof(Aluno)); return; }
            FileResult? arquivo = escolha == "Galeria" ? await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Escolha uma foto", FileTypes = FilePickerFileType.Images }) : escolha == "Câmera" ? await MediaPicker.Default.CapturePhotoAsync() : null;
            if (arquivo is null) return;
            await using var stream = await arquivo.OpenReadAsync(); using var memoria = new MemoryStream(); await stream.CopyToAsync(memoria);
            Aluno.Foto = new ArquivoDto { Conteudo = memoria.ToArray() }; OnPropertyChanged(nameof(Aluno));
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Foto", $"Não foi possível obter a foto: {ex.Message}", "OK"); }
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        try
        {
            Aluno.Cpf = ApenasDigitos(Aluno.Cpf); Aluno.Telefone = ApenasDigitos(Aluno.Telefone);
            if (string.IsNullOrWhiteSpace(Aluno.Nome) || Aluno.Cpf.Length != 11 || Aluno.Telefone.Length < 10 || Aluno.Endereco is null || Aluno.Endereco.Id == 0 || string.IsNullOrWhiteSpace(Aluno.Numero))
                throw new InvalidOperationException("Preencha nome, CPF, telefone, CEP cadastrado e número do endereço.");
            if (!Editando && string.IsNullOrWhiteSpace(Aluno.Senha)) throw new InvalidOperationException("Informe uma senha para o novo aluno.");
            if (!string.IsNullOrWhiteSpace(Aluno.Senha) && Aluno.Senha != ConfirmarSenha) throw new InvalidOperationException("A confirmação da senha não confere.");
            if (Editando) await service.AtualizarAsync(Aluno); else await service.AdicionarAsync(Aluno);
            await Shell.Current.DisplayAlertAsync("Sucesso", "Aluno salvo com sucesso.", "OK");
            await Shell.Current.GoToAsync("//alunos");
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", ex.Message, "OK"); }
    }

    [RelayCommand] private Task CancelarAsync() => Shell.Current.GoToAsync("//alunos");
    private static string ApenasDigitos(string? texto) => new((texto ?? string.Empty).Where(char.IsDigit).ToArray());
}
