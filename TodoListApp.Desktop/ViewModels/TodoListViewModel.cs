using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;
using TodoListApp.Desktop.Infrastructure;
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

    // Сохранения задач выполняются строго по очереди: иначе параллельные запросы могут записать
    // устаревшие данные поверх новых и задвоить строки истории изменений
    private readonly SemaphoreSlim saveLock = new(1, 1);

    private bool isMessagesRegistered = false;

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
        window.Owner = Application.Current.MainWindow;
        window.DataContext = new AddEmployeerWindowViewModel();
        window.ShowDialog();
    }

    [RelayCommand]
    private void ShowLogsWindow()
    {
        var window = new SettingsView();
        window.Owner = Application.Current.MainWindow;
        window.DataContext = settingsViewModel;
        window.ShowDialog();
    }

    [RelayCommand]
    private async Task RemoveEmployeerTab()
    {
        var tab = SelectedTab;
        if (tab == null) { return; }

        var result = MessageBox.Show($"Удалить вкладку '{tab.Employeer.Name}' ?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) { return; }

        try
        {
            await employeerRepository.DeleteEmployeer(tab.Employeer.Id);
            Tabs.Remove(tab);
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, $"Не удалось удалить вкладку '{tab.Employeer.Name}'");
        }
    }

    [RelayCommand]
    private async Task Loaded()
    {
        // Loaded у UserControl может сработать повторно, а повторная регистрация получателя бросает исключение
        if (!isMessagesRegistered)
        {
            WeakReferenceMessenger.Default.RegisterAll(this);
            isMessagesRegistered = true;
        }

        try
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
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось загрузить список работодателей");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Receive(JobItemFieldUpdatedMessage message)
    {
        _ = SaveJobItemAsync(message.Value);
    }

    private async Task SaveJobItemAsync(JobItemViewModel item)
    {
        await saveLock.WaitAsync();
        try
        {
            // Сущность собирается уже внутри очереди, чтобы уйти на сервер с самыми свежими значениями
            await jobItemRepository.AddOrUpdateJobItem(item.ToEntity());
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, $"Не удалось сохранить задачу '{item.Title}'");
        }
        finally
        {
            saveLock.Release();
        }
    }

    public async void Receive(JobItemHistoryDisplayMessage message)
    {
        try
        {
            var window = new JobItemChangesHistoryWindow();
            window.Owner = Application.Current.MainWindow;
            var windowViewModel = new JobItemChangesHistoryWindowViewModel();

            var historyItems = await jobItemRepository.GetHistoryChangesForJobItem(message.Value.Id);
            foreach (var item in historyItems.Select(i => i.ToModel()))
            {
                windowViewModel.HistoryItems.Add(item);
            }

            window.DataContext = windowViewModel;

            window.ShowDialog();
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось загрузить историю изменений");
        }
    }

    public async void Receive(EmployeerCreatedMessage message)
    {
        try
        {
            var emp = new EmployeerEntity() { name = message.newEmployeerName.Trim() };
            var id = await employeerRepository.AddEmployeer(emp);
            emp.id = id;

            var newTab = todoListTabFactory(emp);
            Tabs.Add(newTab);
            SelectedTab = newTab;
        }
        catch (Exception ex)
        {
            // Например, 409 - работодатель с таким именем уже существует
            UiErrors.Show(ex, "Не удалось создать вкладку");
        }
    }
}
