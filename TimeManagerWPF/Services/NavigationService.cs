using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Services;

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

    private readonly Lazy<Dictionary<ViewModelType, ViewModelBase>> availableViewModels;

    public void MoveTo(ViewModelType type) => ActiveViewModel = availableViewModels.Value[type];

    public NavigationLocator(IHost host)
    {
        availableViewModels = new Lazy<Dictionary<ViewModelType, ViewModelBase>>(() => new Dictionary<ViewModelType, ViewModelBase>()
        {
            [ViewModelType.TodoList] = host.Services.GetService<TodoListViewModel>(),
            [ViewModelType.AddEmployeer] = host.Services.GetService<AddEmployeerViewModel>(),
            [ViewModelType.ShoppingList] = host.Services.GetService<ShoppingListViewModel>(),
        });
    }
}

public enum ViewModelType
{
    TodoList,
    AddEmployeer,
    ShoppingList
}
