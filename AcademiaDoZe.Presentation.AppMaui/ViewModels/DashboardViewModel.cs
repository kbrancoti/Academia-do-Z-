// Kaio Fernandes Branco
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;
public partial class DashboardViewModel(ILogradouroService logradouros, IAlunoService alunos, IColaboradorService colaboradores, IMatriculaService matriculas) : BaseViewModel
{
    [ObservableProperty] private int totalLogradouros; [ObservableProperty] private int totalAlunos; [ObservableProperty] private int totalColaboradores; [ObservableProperty] private int totalMatriculas;
    [RelayCommand] public async Task CarregarAsync() { if (IsBusy) return; try { IsBusy = true; TotalLogradouros = (await logradouros.ObterTodosAsync()).Count(); TotalAlunos = (await alunos.ObterTodosAsync()).Count(); TotalColaboradores = (await colaboradores.ObterTodosAsync()).Count(); TotalMatriculas = (await matriculas.ObterTodasAsync()).Count(); } finally { IsBusy = false; } }
    [RelayCommand] private Task AbrirLogradourosAsync() => Shell.Current.GoToAsync("//logradouros");
    [RelayCommand] private Task AbrirAlunosAsync() => Shell.Current.GoToAsync("//alunos");
    [RelayCommand] private Task NovoAlunoAsync() => Shell.Current.GoToAsync("aluno");
    [RelayCommand] private Task NovoColaboradorAsync() => Shell.Current.GoToAsync("colaborador");
    [RelayCommand] private Task AbrirColaboradoresAsync() => Shell.Current.GoToAsync("//colaboradores");
}
