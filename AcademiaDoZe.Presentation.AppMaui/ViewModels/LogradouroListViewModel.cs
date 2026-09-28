// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;
public partial class LogradouroListViewModel(ILogradouroService service) : BaseViewModel
{
    public ObservableCollection<LogradouroDto> Logradouros { get; } = []; [ObservableProperty] private string busca = string.Empty;
    [RelayCommand] public async Task CarregarAsync() { if (IsBusy) return; try { IsBusy = true; var itens = await service.ObterTodosAsync(); Logradouros.Clear(); foreach (var item in itens.Where(x => string.IsNullOrWhiteSpace(Busca) || x.Nome.Contains(Busca, StringComparison.OrdinalIgnoreCase) || x.Cidade.Contains(Busca, StringComparison.OrdinalIgnoreCase))) Logradouros.Add(item); } catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", ex.Message, "OK"); } finally { IsBusy = false; } }
    [RelayCommand] private Task NovoAsync() => Shell.Current.GoToAsync("logradouro");
    [RelayCommand] private Task VoltarInicioAsync() => Shell.Current.GoToAsync("//dashboard");
    [RelayCommand] private Task EditarAsync(LogradouroDto item) => Shell.Current.GoToAsync($"logradouro?id={item.Id}");
    [RelayCommand] private async Task ExcluirAsync(LogradouroDto item) { if (!await Shell.Current.DisplayAlertAsync("Excluir", $"Excluir {item.Nome}?", "Sim", "Não")) return; await service.RemoverAsync(item.Id); await CarregarAsync(); }
}
