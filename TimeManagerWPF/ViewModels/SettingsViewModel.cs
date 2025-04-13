using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using TodoList.WPF.Services;
using TodoListApp.DataAccess.Entities;

namespace TodoList.WPF.ViewModels;

public partial class SettingsViewModel(UserManager userManager) : ObservableObject
{
    [ObservableProperty]
    private bool isLoaded = false;

    [ObservableProperty]
    private bool isTopmost = false;

    [ObservableProperty]
    private IEnumerable<LogEntryEntity> logEntries;

    [RelayCommand]
    private void Loaded()
    {
        LogEntries = userManager.LogEntries;
        IsLoaded = true;
    }
}
