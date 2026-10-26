using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoListApp.Desktop.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Список дел";

    [ObservableProperty]
    private TodoListViewModel todoListViewModel;

    /// <summary>Настройки окна (в т.ч. "Поверх всех окон"), привязаны к Topmost главного окна</summary>
    public AppSettingsViewModel Settings { get; }

    public MainWindowViewModel(TodoListViewModel todoListViewModel, AppSettingsViewModel settings)
    {
        Settings = settings;
        TodoListViewModel = todoListViewModel;
    }
}
