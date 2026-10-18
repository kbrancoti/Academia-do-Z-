// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class AlunoListViewModel(IAlunoService service) : BaseViewModel
{
    public ObservableCollection<AlunoDto> Alunos { get; } = [];
    [ObservableProperty] private string busca = string.Empty;

    [RelayCommand]
    public async Task CarregarAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var filtro = Busca.Trim();
            var alunos = await service.ObterTodosAsync();
            Alunos.Clear();
            foreach (var aluno in alunos.Where(a => string.IsNullOrWhiteSpace(filtro) || a.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase) || a.Cpf.Contains(filtro) || (a.Email?.Contains(filtro, StringComparison.OrdinalIgnoreCase) ?? false)))
                Alunos.Add(aluno);
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", ex.Message, "OK"); }
        finally { IsBusy = false; }
    }

    [RelayCommand] private Task NovoAsync() => Shell.Current.GoToAsync("aluno");
    [RelayCommand] private Task VoltarInicioAsync() => Shell.Current.GoToAsync("//dashboard");
    [RelayCommand] private Task EditarAsync(AlunoDto item) => Shell.Current.GoToAsync($"aluno?id={item.Id}");
    [RelayCommand] private async Task ExcluirAsync(AlunoDto item)
    {
        if (await Shell.Current.DisplayAlertAsync("Excluir aluno", $"Deseja excluir {item.Nome}?", "Sim", "Não")) { await service.RemoverAsync(item.Id); await CarregarAsync(); }
    }
}
