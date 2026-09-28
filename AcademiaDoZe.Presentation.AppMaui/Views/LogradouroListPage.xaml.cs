// Kaio Fernandes Branco
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
namespace AcademiaDoZe.Presentation.AppMaui.Views;
public partial class LogradouroListPage : ContentPage { private readonly LogradouroListViewModel _viewModel; public LogradouroListPage(LogradouroListViewModel viewModel) { InitializeComponent(); BindingContext = _viewModel = viewModel; } protected override async void OnAppearing() { base.OnAppearing(); await _viewModel.CarregarCommand.ExecuteAsync(null); } }
