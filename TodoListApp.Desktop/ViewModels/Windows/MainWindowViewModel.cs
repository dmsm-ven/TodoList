using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;

namespace TodoListApp.Desktop.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private TodoListViewModel todoListViewModel;

    Stopwatch loadingTime;

    public MainWindowViewModel(TodoListViewModel todoListViewModel)
    {
        loadingTime = Stopwatch.StartNew();
        TodoListViewModel = todoListViewModel;
        TodoListViewModel.PropertyChanged += TodoListViewModel_PropertyChanged;
    }

    private void TodoListViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TodoListViewModel.IsLoading) && !TodoListViewModel.IsLoading)
        {
            loadingTime.Stop();
            Title = $"Список дел | Загрузка выполнена за {(int)loadingTime.Elapsed.TotalMilliseconds} мс.";
        }
    }
}
