// Kaio Fernandes Branco
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;
public partial class LogradouroViewModel(ILogradouroService service) : BaseViewModel, IQueryAttributable
{
    [ObservableProperty] private LogradouroDto logradouro = Novo(); [ObservableProperty] private bool editando;
    private static LogradouroDto Novo() => new() { Cep = "", Nome = "", Bairro = "", Cidade = "", Estado = "", Pais = "Brasil" };
    public async void ApplyQueryAttributes(IDictionary<string, object> query) { if (query.TryGetValue("id", out var id) && int.TryParse(id.ToString(), out var value)) { var item = await service.ObterPorIdAsync(value); if (item is not null) { Logradouro = item; Editando = true; Title = "Editar Logradouro"; } } else { Logradouro = Novo(); Editando = false; Title = "Novo Logradouro"; } }
    [RelayCommand] private async Task SalvarAsync() { try { if (string.IsNullOrWhiteSpace(Logradouro.Cep) || string.IsNullOrWhiteSpace(Logradouro.Nome) || string.IsNullOrWhiteSpace(Logradouro.Bairro) || string.IsNullOrWhiteSpace(Logradouro.Cidade) || string.IsNullOrWhiteSpace(Logradouro.Estado)) { await Shell.Current.DisplayAlertAsync("Atenção", "Preencha todos os campos obrigatórios.", "OK"); return; } if (Editando) await service.AtualizarAsync(Logradouro); else await service.AdicionarAsync(Logradouro); await Shell.Current.DisplayAlertAsync("Sucesso", "Logradouro salvo.", "OK"); await Shell.Current.GoToAsync("//logradouros"); } catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Erro", ex.Message, "OK"); } }
    [RelayCommand] private Task CancelarAsync() => Shell.Current.GoToAsync("//logradouros");
}
