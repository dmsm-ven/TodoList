using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TodoListApp.ApiClient;
using TodoListApp.Core.Models;
using TodoListApp.Desktop.Infrastructure;

namespace TodoListApp.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly TodoListAppApiClient appLogger;
    [ObservableProperty]
    private bool isLoaded = false;

    /// <summary>"Поверх всех окон" - применяется к главному окну (см. MainWindow.xaml)</summary>
    [ObservableProperty]
    private bool isTopmost = false;

    [ObservableProperty]
    private ObservableCollection<LogEntryModel> logEntries = new();

    public SettingsViewModel(TodoListAppApiClient appLogger)
    {
        this.appLogger = appLogger;
    }

    [RelayCommand]
    private async Task Loaded()
    {
        try
        {
            var items = await appLogger.GetLastRows(100);

            // VM - синглтон, окно журнала открывается многократно: без Clear строки дублировались бы
            LogEntries.Clear();
            foreach (var item in items.OrderByDescending(i => i.id).Select(i => LogEntryModel.FromEntity(i)))
            {
                LogEntries.Add(item);
            }
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось загрузить журнал");
        }
        finally
        {
            IsLoaded = true;
        }
    }
}
