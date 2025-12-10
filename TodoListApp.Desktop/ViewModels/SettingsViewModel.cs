using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using TodoListApp.Desktop.Services;
using TodoListApp.Core.Entities;
using System;
using TodoListApp.Core.Repositories.Interfaces;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using TodoListApp.Core.Models;

namespace TodoListApp.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IAppLogger appLogger;
    [ObservableProperty]
    private bool isLoaded = false;

    [ObservableProperty]
    private bool isTopmost = false;

    [ObservableProperty]
    private ObservableCollection<LogEntryModel> logEntries = new();

    public SettingsViewModel(IAppLogger appLogger)
    {
        this.appLogger = appLogger;
    }

    [RelayCommand]
    private async Task Loaded()
    {
        var items = await appLogger.GetLastRows(100);

        foreach(var item in items.OrderByDescending(i => i.id).Select(i => LogEntryModel.FromEntity(i)))
        {
            LogEntries.Add(item);
        }
        IsLoaded = true;
    }
}
