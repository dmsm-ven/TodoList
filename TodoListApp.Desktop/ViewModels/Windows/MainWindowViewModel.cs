using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoListApp.Desktop.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private TodoListViewModel todoListViewModel;

    /// <summary>Настройки окна (в т.ч. "Поверх всех окон"), привязаны к Topmost главного окна</summary>
    public SettingsViewModel Settings { get; }

    public MainWindowViewModel(TodoListViewModel todoListViewModel, SettingsViewModel settings)
    {
        TodoListViewModel = todoListViewModel;
        Settings = settings;
    }
}
