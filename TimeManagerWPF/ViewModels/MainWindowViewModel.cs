using System.Windows;
using System.Windows.Input;

namespace TodoList.WPF.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public TodoListViewModel TodoListViewModel { get; } = new TodoListViewModel();
    public CommandToolPanelViewModel CommandToolPanelViewModel { get; } = new CommandToolPanelViewModel();
    public StatusToolPanelViewModel StatusToolPanelViewModel { get; } = new StatusToolPanelViewModel();
    public MainWindowViewModel()
    {

    }
    public MainWindowViewModel(TodoListViewModel todoListViewModel, 
        CommandToolPanelViewModel commandToolPanelViewModel,
        StatusToolPanelViewModel statusToolPanelViewModel) : this()
    {
        TodoListViewModel = todoListViewModel;
        CommandToolPanelViewModel = commandToolPanelViewModel;
        StatusToolPanelViewModel = statusToolPanelViewModel;
    }
}
