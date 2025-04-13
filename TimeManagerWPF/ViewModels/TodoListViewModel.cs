using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TodoList.WPF.Models.TodoList;
using TodoList.WPF.Views;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoList.WPF.ViewModels;

public partial class TodoListViewModel(IEmployeerRepository employeerRepository,
        IEmployeerPaymentRepository employeerPaymentRepository,
        IJobItemRepository jobItemRepository,
        Func<EmployeerEntity, EmployeerTabViewModel> todoListTabFactory) : ObservableObject
{
    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty]
    private ObservableCollection<EmployeerTabViewModel> tabs = new();

    [ObservableProperty]
    private EmployeerTabViewModel selectedTab;

    [ObservableProperty]
    private TodoListTabStatusBarViewModel statusBarViewModel = new();

    partial void OnSelectedTabChanged(EmployeerTabViewModel value)
    {
        StatusBarViewModel.SetSourceItems(value.FilteredTodoItems);
    }

    [RelayCommand]
    private void AddNewEmployeerTab()
    {
        var window = new AddEmployeerWindow();
        window.DataContext = new AddEmployeerWindowViewModel();
        window.ShowDialog();
    }

    [RelayCommand]
    private async Task Loaded()
    {
        var employeers = await Task.Run(() => employeerRepository.GetAllEmployeer());

        foreach (var emp in employeers)
        {
            var newTab = todoListTabFactory(emp);
            Tabs.Add(newTab);
        }

        if (Tabs.Any())
        {
            SelectedTab = Tabs.FirstOrDefault();
        }

        IsLoading = false;
    }
}
