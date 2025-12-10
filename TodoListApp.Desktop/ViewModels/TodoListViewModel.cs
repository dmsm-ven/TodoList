using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TodoList.WPF.Infrastructure.MapperHelper;
using TodoList.WPF.Models.Messages;
using TodoList.WPF.Models.TodoList;
using TodoList.WPF.ViewModels.Windows;
using TodoList.WPF.Views;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoList.WPF.ViewModels;

public partial class TodoListViewModel : ObservableRecipient,
    IRecipient<JobItemFieldUpdatedMessage>,
    IRecipient<JobItemHistoryDisplayMessage>
{

    private readonly IEmployeerRepository employeerRepository;
    private readonly IEmployeerPaymentRepository employeerPaymentRepository;
    private readonly IJobItemRepository jobItemRepository;
    private readonly Func<EmployeerEntity, EmployeerTabViewModel> todoListTabFactory;

    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty]
    private ObservableCollection<EmployeerTabViewModel> tabs = new();

    [ObservableProperty]
    private EmployeerTabViewModel? selectedTab;

    async partial void OnSelectedTabChanged(EmployeerTabViewModel? oldValue, EmployeerTabViewModel? newValue)
    {
        if (newValue != null)
        {
            await newValue.LoadedCommand.ExecuteAsync(null);
        }
    }

    public TodoListViewModel(IEmployeerRepository employeerRepository,
        IEmployeerPaymentRepository employeerPaymentRepository,
        IJobItemRepository jobItemRepository,
        Func<EmployeerEntity, EmployeerTabViewModel> todoListTabFactory)
    {
        this.employeerRepository = employeerRepository;
        this.employeerPaymentRepository = employeerPaymentRepository;
        this.jobItemRepository = jobItemRepository;
        this.todoListTabFactory = todoListTabFactory;
    }

    [RelayCommand]
    private void AddNewEmployeerTab()
    {
        var window = new AddEmployeerWindow();
        window.DataContext = new AddEmployeerWindowViewModel();
        window.ShowDialog();
    }

    [RelayCommand]
    private void RemoveEmployeerTab()
    {
        var result = MessageBox.Show($"Удалить вкладку '{SelectedTab.Employeer.Name}' ?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            employeerRepository.DeleteEmployeer(SelectedTab.Employeer.Id);
            Tabs.Remove(SelectedTab);
        }
    }

    [RelayCommand]
    private async Task Loaded()
    {
        var employeers = await employeerRepository.GetAllEmployeer();

        foreach (var emp in employeers)
        {
            var newTab = todoListTabFactory(emp);
            Tabs.Add(newTab);
        }

        if (Tabs.Any())
        {
            SelectedTab = Tabs.FirstOrDefault();
        }

        WeakReferenceMessenger.Default.RegisterAll(this);

        IsLoading = false;
    }

    public void Receive(JobItemFieldUpdatedMessage message)
    {
        jobItemRepository.AddOrUpdateJobItem(message.Value.ToEntity());
        jobItemRepository.AddHistoryChanges(message.Value.Id, message.FieldName, message.FieldValue);
    }

    public async void Receive(JobItemHistoryDisplayMessage message)
    {
        var window = new JobItemChangesHistoryWindow();
        var windowViewModel = new JobItemChangesHistoryWindowViewModel();

        var historyItems = await jobItemRepository.GetHistoryChangesForJobItem(message.Value.Id);
        foreach (var item in historyItems.Select(i => i.ToModel()))
        {
            windowViewModel.HistoryItems.Add(item);
        }

        window.DataContext = windowViewModel;

        window.ShowDialog();
    }
}
