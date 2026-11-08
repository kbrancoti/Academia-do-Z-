// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class MatriculaListViewModel(IMatriculaService service) : BaseViewModel
{
    public ObservableCollection<MatriculaDto> Matriculas { get; } = [];
    [ObservableProperty] private string busca = string.Empty;

    [RelayCommand]
    public async Task CarregarAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var texto = Busca.Trim();
            var itens = await service.ObterTodasAsync();
            Matriculas.Clear();
            foreach (var item in itens.Where(m => string.IsNullOrWhiteSpace(texto) || m.AlunoMatricula.Nome.Contains(texto, StringComparison.OrdinalIgnoreCase) || m.AlunoMatricula.Cpf.Contains(texto)))
                Matriculas.Add(item);
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", ex.Message, "OK"); }
        finally { IsBusy = false; }
    }

    [RelayCommand] private Task NovoAsync() => Shell.Current.GoToAsync("matricula");
    [RelayCommand] private Task VoltarInicioAsync() => Shell.Current.GoToAsync("//dashboard");
    [RelayCommand] private Task EditarAsync(MatriculaDto item) => Shell.Current.GoToAsync($"matricula?id={item.Id}");
    [RelayCommand]
    private async Task ExcluirAsync(MatriculaDto item)
    {
        if (await Shell.Current.DisplayAlertAsync("Excluir matrícula", $"Deseja excluir a matrícula de {item.AlunoMatricula.Nome}?", "Sim", "Não"))
        {
            await service.RemoverAsync(item.Id);
            await CarregarAsync();
        }
    }
}
