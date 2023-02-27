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

    IReadOnlyDictionary<ViewModelType, Lazy<ViewModelBase>> AvailableViewModels { get; }

    public void MoveTo(ViewModelType type)
    {
        try
        {
            ActiveViewModelType = type;
            ActiveViewModel = AvailableViewModels[type].Value;
        }
        catch (Exception ex)
        {
            ActiveViewModelType = ViewModelType.ErrorView;
            ActiveViewModel = AvailableViewModels[ActiveViewModelType].Value;
            (ActiveViewModel as ConnectionErrorWindowViewModel).ErrorMessage = ex.Message;
        }

    }

    public NavigationLocator(IHost host)
    {
        AvailableViewModels = new Dictionary<ViewModelType, Lazy<ViewModelBase>>()
        {
            [ViewModelType.TodoList] = new Lazy<ViewModelBase>(() => host.Services.GetService<TodoListViewModel>()),
            [ViewModelType.ShoppingList] = new Lazy<ViewModelBase>(() => host.Services.GetService<ShoppingListViewModel>()),
            [ViewModelType.ReadList] = new Lazy<ViewModelBase>(() => host.Services.GetService<ReadListViewModel>()),
            [ViewModelType.ErrorView] = new Lazy<ViewModelBase>(() => host.Services.GetService<ConnectionErrorWindowViewModel>()),
            [ViewModelType.SettingsView] = new Lazy<ViewModelBase>(() => host.Services.GetService<SettingsViewModel>()),
            [ViewModelType.BudgetView] = new Lazy<ViewModelBase>(() => host.Services.GetService<BudgetViewModel>())
        };
    }
}
