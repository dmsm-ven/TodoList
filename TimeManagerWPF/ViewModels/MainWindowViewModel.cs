using System;
using System.Windows;
using System.Windows.Input;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public NavigationLocator NavigationLocator { get; }
    public TodoListViewModel TodoListViewModel { get; }
    public MainWindowViewModel()
    {

    }
    public MainWindowViewModel(TodoListViewModel todoListViewModel, NavigationLocator navigationLocator) : this()
    {
        TodoListViewModel = todoListViewModel;
        NavigationLocator = navigationLocator;
    }
}
