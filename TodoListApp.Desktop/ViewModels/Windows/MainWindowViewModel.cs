using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoList.WPF.Models;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private TodoListViewModel todoListViewModel;

    public MainWindowViewModel(TodoListViewModel todoListViewModel)
    {
        TodoListViewModel = todoListViewModel;
    }
}
