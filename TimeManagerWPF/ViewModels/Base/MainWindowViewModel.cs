using System;
using System.Windows;
using System.Windows.Input;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public NavigationLocator NavigationLocator { get; }
    public TodoListViewModel TodoListViewModel { get; }
    public ToolPanelViewModel ToolPanelViewModel { get; }
    
    public ICommand LoadedCommand { get; }

    public MainWindowViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
    }

    public MainWindowViewModel(TodoListViewModel todoListViewModel, 
        ToolPanelViewModel toolPanelViewModel,
        NavigationLocator navigationLocator) : this()
    {
        TodoListViewModel = todoListViewModel;
        NavigationLocator = navigationLocator; 
        ToolPanelViewModel = toolPanelViewModel;
    }

    private void Loaded(object obj)
    {
        NavigationLocator.MoveTo(ViewModelType.TodoList);
    }

}
