using System.Windows;
using System.Windows.Input;

namespace TodoList.WPF.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public TodoListViewModel TodoListViewModel { get; } 
    public ICommand ShowSettingsCommand { get; }
    public ICommand AddJobItemCommand { get; }
    public ICommand DeleteJobItemCommand { get; }

    public MainWindowViewModel()
    {
        ShowSettingsCommand = new LambdaCommand(e => { MessageBox.Show("NOT IMPLEMENTED"); });
        AddJobItemCommand = new LambdaCommand(AddJobItem);
        DeleteJobItemCommand = new LambdaCommand(DeleteJobItem, e => TodoListViewModel?.SelectedTab?.SelectedJobItem != null);
    }
    public MainWindowViewModel(TodoListViewModel todoListViewModel) : this()
    {
        TodoListViewModel = todoListViewModel;
    }

    private void DeleteJobItem(object obj)
    {
        var answer = MessageBox.Show("Удалить выделенное задание ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer == MessageBoxResult.Yes)
        {
            TodoListViewModel.SelectedTab.DeleteJobItem();
        }
    }

    private void AddJobItem(object obj)
    {
        TodoListViewModel.SelectedTab.AddJobItem();
    }
}
