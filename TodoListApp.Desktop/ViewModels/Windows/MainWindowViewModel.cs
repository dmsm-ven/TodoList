using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoListApp.Desktop.Models;
using TodoListApp.Desktop.Services;

namespace TodoListApp.Desktop.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private TodoListViewModel todoListViewModel;

    public MainWindowViewModel(TodoListViewModel todoListViewModel)
    {
        TodoListViewModel = todoListViewModel;
    }
}
