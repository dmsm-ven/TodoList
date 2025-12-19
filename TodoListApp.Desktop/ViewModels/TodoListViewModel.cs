using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;
using TodoListApp.Desktop.Infrastructure.MapperHelper;
using TodoListApp.Desktop.Models.Messages;
using TodoListApp.Desktop.Models.TodoList;
using TodoListApp.Desktop.ViewModels.Windows;
using TodoListApp.Desktop.Views;

namespace TodoListApp.Desktop.ViewModels;

public partial class TodoListViewModel : ObservableRecipient,
    IRecipient<JobItemFieldUpdatedMessage>,
    IRecipient<JobItemHistoryDisplayMessage>,
    IRecipient<EmployeerCreatedMessage>
{

    private readonly IEmployeerRepository employeerRepository;
    private readonly IJobItemRepository jobItemRepository;
    private readonly SettingsViewModel settingsViewModel;
    private readonly Func<EmployeerEntity, EmployeerTabViewModel> todoListTabFactory;

    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty]
    private ObservableCollection<EmployeerTabViewModel> tabs = new();

    [ObservableProperty]
    private EmployeerTabViewModel? selectedTab = null;
    async partial void OnSelectedTabChanged(EmployeerTabViewModel? oldValue, EmployeerTabViewModel? newValue)
    {
        if (newValue != null)
        {
            await newValue.LoadedCommand.ExecuteAsync(null);
        }
    }

    public TodoListViewModel(IEmployeerRepository employeerRepository,
        IJobItemRepository jobItemRepository,
        SettingsViewModel settingsViewModel,
        Func<EmployeerEntity, EmployeerTabViewModel> todoListTabFactory)
    {
        this.employeerRepository = employeerRepository;
        this.jobItemRepository = jobItemRepository;
        this.settingsViewModel = settingsViewModel;
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
    private void ShowLogsWindow()
    {
        var window = new SettingsView();
        window.DataContext = settingsViewModel;
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
        this.Tabs.Clear();

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

    public async void Receive(EmployeerCreatedMessage message)
    {
        var emp = new EmployeerEntity() { name = message.newEmployeerName };
        var id = await employeerRepository.AddEmployeer(emp);
        emp.id = id;

        var newTab = todoListTabFactory(emp);
        Tabs.Add(newTab);
        SelectedTab = newTab;

    }
}
