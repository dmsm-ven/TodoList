using CommunityToolkit.Mvvm.ComponentModel;

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
