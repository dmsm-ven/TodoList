using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using TodoListApp.Desktop.Services;
using TodoListApp.Core.Entities;
using System;

namespace TodoListApp.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
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
        //LogEntries = 
        throw new NotImplementedException("тут вызов API");
        IsLoaded = true;
    }
}
