using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF;

public class NavigationLocator : ViewModelBase
{
    public event Action ActiveViewModelChanged;

    ViewModelBase activeViewModel;
    public ViewModelBase ActiveViewModel
    {
        get => activeViewModel;
        private set
        {
            if (Set(ref activeViewModel, value))
            {
                ActiveViewModelChanged?.Invoke();
            }
        }
    }

    ViewModelType activeViewModelType;
    public ViewModelType ActiveViewModelType
    {
        get => activeViewModelType;
        private set => Set(ref activeViewModelType, value);
    }

    private readonly Lazy<Dictionary<ViewModelType, ViewModelBase>> availableViewModels;

    public void MoveTo(ViewModelType type)
    {
        try
        {
            ActiveViewModelType = type;
            ActiveViewModel = availableViewModels.Value[type];
        }
        catch (Exception ex)
        {
            ActiveViewModelType = ViewModelType.ErrorView;
            ActiveViewModel = availableViewModels.Value[ActiveViewModelType];
            (ActiveViewModel as ConnectionErrorWindowViewModel).ErrorMessage = ex.Message;
        }

    }

    public NavigationLocator(IHost host)
    {
        availableViewModels = new Lazy<Dictionary<ViewModelType, ViewModelBase>>(() =>
            new Dictionary<ViewModelType, ViewModelBase>()
            {
                [ViewModelType.TodoList] = host.Services.GetService<TodoListViewModel>(),
                [ViewModelType.ShoppingList] = host.Services.GetService<ShoppingListViewModel>(),
                [ViewModelType.ReadList] = host.Services.GetService<ReadListViewModel>(),
                [ViewModelType.ErrorView] = host.Services.GetService<ConnectionErrorWindowViewModel>(),
                [ViewModelType.SettingsView] = host.Services.GetService<SettingsViewModel>(),
                [ViewModelType.BudgetView] = host.Services.GetService<BudgetViewModel>(),
            });
    }
}
