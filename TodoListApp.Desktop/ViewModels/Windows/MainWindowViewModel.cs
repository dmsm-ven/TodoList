using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;

namespace TodoListApp.Desktop.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "";

    [ObservableProperty]
    private TodoListViewModel todoListViewModel;

    Stopwatch loadingTime;

    /// <summary>Настройки окна (в т.ч. "Поверх всех окон"), привязаны к Topmost главного окна</summary>
    public SettingsViewModel Settings { get; }

    public MainWindowViewModel(TodoListViewModel todoListViewModel, SettingsViewModel settings)
    {
        loadingTime = Stopwatch.StartNew();
        Settings = settings;
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
