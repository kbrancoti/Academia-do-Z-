// Kaio Fernandes Branco
using CommunityToolkit.Mvvm.ComponentModel;
namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;
public partial class BaseViewModel : ObservableObject { [ObservableProperty] private bool isBusy; [ObservableProperty] private string title = string.Empty; }
