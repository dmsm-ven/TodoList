using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoList.WPF.Models;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels;

public partial class ToolPanelViewModel : ObservableObject
{
    private readonly NavigationLocator navigationLocator;

    [RelayCommand]
    private void MoveToTodoList() => navigationLocator.Navigate(ViewModelType.TodoList);

    [RelayCommand]
    private void AddEmployeer() => navigationLocator.Navigate(ViewModelType.AddEmployeer);

    [RelayCommand]
    private void MoveToSettingsView() => navigationLocator.Navigate(ViewModelType.SettingsView);

    public ToolPanelViewModel(NavigationLocator navigationLocator)
    {
        this.navigationLocator = navigationLocator;
    }
}
