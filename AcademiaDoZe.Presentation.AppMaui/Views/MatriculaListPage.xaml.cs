// Kaio Fernandes Branco
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class MatriculaListPage : ContentPage
{
    private readonly MatriculaListViewModel _viewModel;
    public MatriculaListPage(MatriculaListViewModel viewModel) { InitializeComponent(); BindingContext = _viewModel = viewModel; }
    protected override async void OnAppearing() { base.OnAppearing(); await _viewModel.CarregarCommand.ExecuteAsync(null); }
}
