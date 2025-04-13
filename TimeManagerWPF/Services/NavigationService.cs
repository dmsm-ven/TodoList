using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TodoList.WPF.Models;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Services;

public partial class NavigationLocator : ObservableObject
{
    private readonly TodoListViewModel todoListViewModel;
    private readonly SettingsViewModel settingsViewModel;

    [ObservableProperty] private ObservableObject activeViewModel;

    public NavigationLocator(TodoListViewModel todoListViewModel,
    SettingsViewModel settingsViewModel)
    {
        this.todoListViewModel = todoListViewModel;
        this.settingsViewModel = settingsViewModel;

        ActiveViewModel = todoListViewModel;
    }

    public void Navigate(ViewModelType type)
    {
        ActiveViewModel = type switch
        {
            ViewModelType.TodoList => todoListViewModel,
            ViewModelType.SettingsView => settingsViewModel,
            _ => throw new NotImplementedException()
        };
    }
}
