using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TodoListApp.ApiClient;
using TodoListApp.Desktop.Infrastructure;
using TodoListApp.Desktop.Infrastructure.MapperHelper;
using TodoListApp.Desktop.Models.Messages;
using TodoListApp.Desktop.ViewModels;

namespace TodoListApp.Desktop.Models.TodoList;

public partial class EmployeerTabViewModel : ObservableRecipient,
    IRecipient<JobItemFieldUpdatedMessage>,
    IRecipient<MonthPillSelectionChangedMessage>
{
    private readonly TodoListAppApiClient apiClient;
    private bool isLoaded = false;

    // true во время начальной загрузки: не пересчитываем список после каждого добавленного элемента
    private bool isBulkLoading = false;

    public EmployeerViewModel Employeer { get; private set; }

    private const int InitialMonthPillsCount = 6;

    public ObservableCollection<MonthPillViewModel> MonthPills { get; init; } = new();

    // Все месяцы, в которых есть задачи; в MonthPills показываются только последние InitialMonthPillsCount, пока не нажато "Показать еще"
    private List<MonthPillViewModel> allMonthPills = new();

    [ObservableProperty]
    private bool hasHiddenMonthPills;

    [RelayCommand]
    private void ShowMoreMonthPills()
    {
        foreach (var pill in allMonthPills.Skip(MonthPills.Count))
        {
            MonthPills.Add(pill);
        }
        HasHiddenMonthPills = false;
    }

    [ObservableProperty]
    private bool isShowPaymentField;

    [ObservableProperty]
    private bool isLoading;

    [NotifyPropertyChangedFor(nameof(FilteredTodoItems))]
    [ObservableProperty]
    private MonthPillViewModel selectedMonthPill;

    partial void OnSelectedMonthPillChanged(MonthPillViewModel value)
    {
        if (value == null) { return; }
        foreach (var item in MonthPills)
        {
            item.IsSelected = false;
        }
        value.IsSelected = true;
        RefreshSource();
    }

    [ObservableProperty]
    private JobItemViewModel selectedJobItem;

    [ObservableProperty]
    private EmployeerPaymentViewModel newPayment;

    [ObservableProperty]
    private IEnumerable<JobItemViewModel> filteredTodoItems;

    [NotifyPropertyChangedFor(nameof(FilteredTodoItems))]
    [ObservableProperty]
    private string searchText;

    partial void OnSearchTextChanged(string value)
    {
        RefreshSource();
    }

    [ObservableProperty]
    private TodoListTabStatusBarViewModel statusBarData = new();

    public bool HasActiveTasks
    {
        get => Employeer.TodoItems?.Any(t => t.IsCompleted == false) ?? false;
    }
    public int ActiveTasksCount
    {
        get => Employeer.TodoItems?.Count(t => !t.IsCompleted || !t.IsPayed) ?? 0;
    }

    public IEnumerable<string> UniqueWebsites
    {
        get
        {
            if (Employeer.TodoItems.Count == 0)
            {
                return new string[] { "https://" };
            }
            return Employeer.TodoItems
                .GroupBy(item => item.Website)
                .Select(g => g.Key)
                .Where(g => g != null)
                .OrderBy(site => site);
        }
    }

    public EmployeerTabViewModel(TodoListAppApiClient apiClient)
    {
        this.apiClient = apiClient;
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void SetEmployeer(EmployeerViewModel emp)
    {
        Employeer = emp;
        this.NewPayment = new EmployeerPaymentViewModel() { EmployeerId = emp.Id, EmployeerName = emp.Name };
        Employeer.TodoItems.CollectionChanged += (o, e) =>
        {
            RefreshSource();
            NotifyActiveTasksChanged();
        };
    }

    [RelayCommand]
    private void ShowPaymentField()
    {
        IsShowPaymentField = !IsShowPaymentField;
    }

    [RelayCommand]
    private async Task Loaded()
    {
        if (isLoaded) { return; }
        isLoaded = true;

        if (Employeer == null)
        {
            throw new InvalidOperationException("Employeer is not set");
        }

        IsLoading = true;

        try
        {
            var jobsTask = apiClient.GetAllJobItems(Employeer.Id);
            var paymentsTask = apiClient.GetAllPaymentsForEmployeer(Employeer.Id);

            await Task.WhenAll(jobsTask, paymentsTask);

            isBulkLoading = true;
            try
            {
                foreach (var job in (await jobsTask).Select(i => i.ToViewModel()))
                {
                    Employeer.TodoItems.Add(job);
                    job.Initialize();
                }
            }
            finally
            {
                isBulkLoading = false;
            }

            foreach (var payment in (await paymentsTask).Select(i => i.ToViewModel()))
            {
                Employeer.Payments.Add(payment);
            }

            Employeer.Initialize();

            LoadMonthPills();

            RefreshSource();

            NotifyActiveTasksChanged();

            WeakReferenceMessenger.Default.Send(new EmployeeTabLoadedMessage(this));
        }
        catch (Exception ex)
        {
            // Позволяем повторить загрузку при следующем выборе вкладки
            isLoaded = false;
            UiErrors.Show(ex, $"Не удалось загрузить данные для '{Employeer.Name}'");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddEmployeerPayment()
    {
        if (NewPayment.Amount <= 0)
        {
            MessageBox.Show("Укажите сумму оплаты больше нуля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var payment = NewPayment;

        try
        {
            await apiClient.AddPayment(payment.ToPayload());
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось сохранить оплату");
            return;
        }

        // В список попадает только то, что реально сохранено на сервере
        Employeer.Payments.Insert(0, payment);
        IsShowPaymentField = false;
        NewPayment = new EmployeerPaymentViewModel() { EmployeerId = Employeer.Id, EmployeerName = Employeer.Name };
    }

    private void LoadMonthPills()
    {
        MonthPills.Clear();
        allMonthPills.Clear();

        var pillsData = Employeer.TodoItems
            .Where(i => i.StartDate.HasValue)
            .Select(i => i.StartDate!.Value)
            .Select(date => new { date.Year, date.Month })
            .GroupBy(i => $"{i.Year}-{i.Month}")
            .Select(i => i.First())
            .OrderByDescending(i => i.Year)
            .ThenByDescending(i => i.Month);

        foreach (var kvp in pillsData)
        {
            var pill = new MonthPillViewModel(this) { MonthNumber = kvp.Month, Year = kvp.Year };
            allMonthPills.Add(pill);
        }

        foreach (var pill in allMonthPills.Take(InitialMonthPillsCount))
        {
            MonthPills.Add(pill);
        }
        HasHiddenMonthPills = allMonthPills.Count > MonthPills.Count;

        SelectedMonthPill = MonthPills?.FirstOrDefault();
    }

    [RelayCommand]
    public async Task AddJobItem()
    {
        try
        {
            var vmItem = new JobItemViewModel()
            {
                StartDate = DateTime.Now,
                Title = "Новая задача",
                EmployeerId = Employeer.Id
            };

            // Сначала сохраняем на сервере: при ошибке в таблице не остается "призрачной" задачи без Id
            var newJob = await apiClient.AddJobItem(vmItem.ToCreateJobItem());
            vmItem.Id = newJob.id;

            // Чтобы новая задача была видна: сбрасываем поиск и переходим на самый свежий месяц
            SearchText = string.Empty;

            Employeer.TodoItems.Add(vmItem);
            vmItem.Initialize();

            LoadMonthPills();
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось добавить задачу");
        }
    }

    [RelayCommand]
    private async Task DeleteSelectedJobItem(JobItemViewModel item)
    {
        if (item == null) { return; }

        var answer = MessageBox.Show($"Удалить выделенное задание ?\r\n'{item.Title}' от [{item.StartDate:dd.MM.yyyy}]", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) { return; }

        try
        {
            await apiClient.DeleteJobItem(item.Id);
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось удалить задачу");
            return;
        }

        Employeer.TodoItems.Remove(item);

        SelectedJobItem = null;
    }

    /// <summary>
    /// Пересчитывает список задач: при непустом поиске - по всем загруженным задачам, иначе - задачи выбранного месяца.
    /// Заодно обновляет статус-бар, чтобы итоги всегда считались по тому, что показано в таблице.
    /// </summary>
    private void RefreshSource()
    {
        if (isBulkLoading || Employeer == null) { return; }

        IEnumerable<JobItemViewModel> source;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var query = SearchText.Trim();

            source = Employeer.TodoItems
                .Where(i => Contains(i.Title, query) || Contains(i.Description, query) || Contains(i.Website, query))
                .OrderByDescending(i => i.StartDate);
        }
        else
        {
            var activePill = SelectedMonthPill ?? MonthPills.FirstOrDefault();
            if (activePill != null)
            {
                source = Employeer.TodoItems
                    .Where(i => i.StartDate.HasValue &&
                                i.StartDate.Value.Month == activePill.MonthNumber &&
                                i.StartDate.Value.Year == activePill.Year)
                    .OrderByDescending(i => i.IsCompleted ? 0 : 1)
                    .ThenByDescending(i => i.StartDate);
            }
            else
            {
                source = Enumerable.Empty<JobItemViewModel>();
            }
        }

        FilteredTodoItems = source.ToList();
        StatusBarData.SetSourceItems(FilteredTodoItems);
    }

    private static bool Contains(string? text, string query)
    {
        return text != null && text.Contains(query, StringComparison.CurrentCultureIgnoreCase);
    }

    private void NotifyActiveTasksChanged()
    {
        OnPropertyChanged(nameof(HasActiveTasks));
        OnPropertyChanged(nameof(ActiveTasksCount));
    }

    public void Receive(JobItemFieldUpdatedMessage message)
    {
        NotifyActiveTasksChanged();
    }

    public void Receive(MonthPillSelectionChangedMessage message)
    {
        if (message.Value.ParentEmployee != this) { return; }

        // Клик по месяцу выходит из режима поиска
        if (!string.IsNullOrEmpty(SearchText))
        {
            SearchText = string.Empty;
        }

        SelectedMonthPill = message.Value;
    }
}
