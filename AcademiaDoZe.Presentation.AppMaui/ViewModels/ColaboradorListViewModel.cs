// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ColaboradorListViewModel(IColaboradorService service) : BaseViewModel
{
    public ObservableCollection<ColaboradorDto> Colaboradores { get; } = [];
    [ObservableProperty] private string busca = string.Empty;

    [RelayCommand]
    public async Task CarregarAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var filtro = Busca.Trim();
            var itens = await service.ObterTodosAsync();
            Colaboradores.Clear();
            foreach (var item in itens.Where(c => string.IsNullOrWhiteSpace(filtro) || c.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase) || c.Cpf.Contains(filtro) || (c.Email?.Contains(filtro, StringComparison.OrdinalIgnoreCase) ?? false)))
                Colaboradores.Add(item);
        }
        catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", ex.Message, "OK"); }
        finally { IsBusy = false; }
    }

    [RelayCommand] private Task NovoAsync() => Shell.Current.GoToAsync("colaborador");
    [RelayCommand] private Task VoltarInicioAsync() => Shell.Current.GoToAsync("//dashboard");
    [RelayCommand] private Task EditarAsync(ColaboradorDto item) => Shell.Current.GoToAsync($"colaborador?id={item.Id}");
    [RelayCommand] private async Task ExcluirAsync(ColaboradorDto item)
    {
        if (await Shell.Current.DisplayAlertAsync("Excluir colaborador", $"Deseja excluir {item.Nome}?", "Sim", "Não")) { await service.RemoverAsync(item.Id); await CarregarAsync(); }
    }
}
