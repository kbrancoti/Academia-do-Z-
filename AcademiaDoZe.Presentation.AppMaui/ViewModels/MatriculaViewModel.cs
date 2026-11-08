// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class MatriculaViewModel(IMatriculaService service, IAlunoService alunoService) : BaseViewModel, IQueryAttributable
{
    public ObservableCollection<AlunoDto> Alunos { get; } = [];
    public IEnumerable<AppMatriculaPlano> Planos => Enum.GetValues<AppMatriculaPlano>();

    [ObservableProperty] private MatriculaDto matricula = Nova();
    [ObservableProperty] private AlunoDto? alunoSelecionado;
    [ObservableProperty] private AppMatriculaPlano planoSelecionado = AppMatriculaPlano.Mensal;
    [ObservableProperty] private DateTime dataInicioSelecionada = DateTime.Today;
    [ObservableProperty] private DateTime dataFimCalculada = DateTime.Today.AddMonths(1);
    [ObservableProperty] private bool editando;
    [ObservableProperty] private bool diabetes;
    [ObservableProperty] private bool pressaoAlta;
    [ObservableProperty] private bool labirintite;
    [ObservableProperty] private bool alergias;
    [ObservableProperty] private bool problemasRespiratorios;
    [ObservableProperty] private bool remedioContinuo;
    [ObservableProperty] private string nomeLaudo = "Nenhum laudo selecionado";

    private static MatriculaDto Nova() => new()
    {
        AlunoMatricula = AlunoVazio(), Plano = AppMatriculaPlano.Mensal,
        DataInicio = DateOnly.FromDateTime(DateTime.Today), DataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
        Objetivo = "", RestricoesMedicas = AppMatriculaRestricoes.None, ObservacoesRestricoes = ""
    };

    private static AlunoDto AlunoVazio() => new()
    {
        Nome = "", Cpf = "", DataNascimento = DateOnly.FromDateTime(DateTime.Today), Telefone = "", Numero = ""
    };

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await CarregarAlunosAsync();
        if (query.TryGetValue("id", out var id) && int.TryParse(id?.ToString(), out var valor))
        {
            var item = await service.ObterPorIdAsync(valor);
            if (item is not null)
            {
                Matricula = item; Editando = true; Title = "Editar Matrícula";
                AlunoSelecionado = Alunos.FirstOrDefault(a => a.Id == item.AlunoMatricula.Id) ?? item.AlunoMatricula;
                PlanoSelecionado = item.Plano; DataInicioSelecionada = item.DataInicio.ToDateTime(TimeOnly.MinValue);
                AplicarRestricoes(item.RestricoesMedicas); NomeLaudo = item.LaudoMedico is null ? "Nenhum laudo selecionado" : "Laudo já anexado";
            }
        }
        else
        {
            Matricula = Nova(); Editando = false; Title = "Nova Matrícula";
            AlunoSelecionado = null; PlanoSelecionado = AppMatriculaPlano.Mensal; DataInicioSelecionada = DateTime.Today;
            AplicarRestricoes(AppMatriculaRestricoes.None); NomeLaudo = "Nenhum laudo selecionado";
        }
        RecalcularDataFim();
    }

    private async Task CarregarAlunosAsync()
    {
        Alunos.Clear();
        foreach (var aluno in await alunoService.ObterTodosAsync()) Alunos.Add(aluno);
    }

    partial void OnAlunoSelecionadoChanged(AlunoDto? value)
    {
        if (value is not null) { Matricula.AlunoMatricula = value; OnPropertyChanged(nameof(Matricula)); }
    }
    partial void OnPlanoSelecionadoChanged(AppMatriculaPlano value) => RecalcularDataFim();
    partial void OnDataInicioSelecionadaChanged(DateTime value) => RecalcularDataFim();
    partial void OnDiabetesChanged(bool value) => AtualizarRestricoes();
    partial void OnPressaoAltaChanged(bool value) => AtualizarRestricoes();
    partial void OnLabirintiteChanged(bool value) => AtualizarRestricoes();
    partial void OnAlergiasChanged(bool value) => AtualizarRestricoes();
    partial void OnProblemasRespiratoriosChanged(bool value) => AtualizarRestricoes();
    partial void OnRemedioContinuoChanged(bool value) => AtualizarRestricoes();

    private void RecalcularDataFim()
    {
        var inicio = DateOnly.FromDateTime(DataInicioSelecionada);
        var fim = PlanoSelecionado switch
        {
            AppMatriculaPlano.Mensal => inicio.AddMonths(1), AppMatriculaPlano.Trimestral => inicio.AddMonths(3),
            AppMatriculaPlano.Semestral => inicio.AddMonths(6), AppMatriculaPlano.Anual => inicio.AddYears(1), _ => inicio
        };
        DataFimCalculada = fim.ToDateTime(TimeOnly.MinValue);
        Matricula.Plano = PlanoSelecionado; Matricula.DataInicio = inicio; Matricula.DataFim = fim;
        OnPropertyChanged(nameof(Matricula));
    }

    private void AplicarRestricoes(AppMatriculaRestricoes value)
    {
        Diabetes = value.HasFlag(AppMatriculaRestricoes.Diabetes); PressaoAlta = value.HasFlag(AppMatriculaRestricoes.PressaoAlta);
        Labirintite = value.HasFlag(AppMatriculaRestricoes.Labirintite); Alergias = value.HasFlag(AppMatriculaRestricoes.Alergias);
        ProblemasRespiratorios = value.HasFlag(AppMatriculaRestricoes.ProblemasRespiratorios); RemedioContinuo = value.HasFlag(AppMatriculaRestricoes.RemedioContinuo);
    }

    private void AtualizarRestricoes()
    {
        var resultado = AppMatriculaRestricoes.None;
        if (Diabetes) resultado |= AppMatriculaRestricoes.Diabetes; if (PressaoAlta) resultado |= AppMatriculaRestricoes.PressaoAlta;
        if (Labirintite) resultado |= AppMatriculaRestricoes.Labirintite; if (Alergias) resultado |= AppMatriculaRestricoes.Alergias;
        if (ProblemasRespiratorios) resultado |= AppMatriculaRestricoes.ProblemasRespiratorios; if (RemedioContinuo) resultado |= AppMatriculaRestricoes.RemedioContinuo;
        Matricula.RestricoesMedicas = resultado; OnPropertyChanged(nameof(Matricula));
    }

    [RelayCommand]
    private async Task SelecionarLaudoAsync()
    {
        try
        {
            var arquivo = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Selecione o laudo médico" });
            if (arquivo is null) return;
            await using var entrada = await arquivo.OpenReadAsync(); using var memoria = new MemoryStream(); await entrada.CopyToAsync(memoria);
            Matricula.LaudoMedico = new ArquivoDto { Conteudo = memoria.ToArray() }; NomeLaudo = arquivo.FileName; OnPropertyChanged(nameof(Matricula));
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Laudo", $"Não foi possível selecionar o arquivo: {ex.Message}", "OK"); }
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        try
        {
            if (AlunoSelecionado is null || AlunoSelecionado.Id == 0) throw new InvalidOperationException("Selecione um aluno.");
            if (string.IsNullOrWhiteSpace(Matricula.Objetivo)) throw new InvalidOperationException("Informe o objetivo da matrícula.");
            RecalcularDataFim(); AtualizarRestricoes();
            if (Editando) await service.AtualizarAsync(Matricula); else await service.AdicionarAsync(Matricula);
            await Shell.Current.DisplayAlertAsync("Sucesso", "Matrícula salva com sucesso.", "OK");
            await Shell.Current.GoToAsync("//matriculas");
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", ex.Message, "OK"); }
    }

    [RelayCommand] private Task CancelarAsync() => Shell.Current.GoToAsync("//matriculas");
}
