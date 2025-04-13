using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoList.WPF.Models;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private NavigationLocator navigationLocator;

    public MainWindowViewModel(NavigationLocator navigationLocator)
    {
        NavigationLocator = navigationLocator;
    }

    [RelayCommand]
    private void Loaded()
    {
        NavigationLocator.Navigate(ViewModelType.TodoList);
    }
}
