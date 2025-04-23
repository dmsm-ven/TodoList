using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TodoList.WPF.Infrastructure.MapperHelper;
using TodoList.WPF.Models.Messages;
using TodoList.WPF.ViewModels;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoList.WPF.Models.TodoList;

public partial class EmployeerTabViewModel : ObservableRecipient,
    IRecipient<JobItemFieldUpdatedMessage>,
    IRecipient<MonthPillSelectionChangedMessage>
{
    public const int MAX_PILLS_COUNT = 12;

    private bool isLoaded = false;
    private readonly IJobItemRepository jobItemRepository;
    private readonly IEmployeerPaymentRepository paymentRepository;

    public EmployeerViewModel Employeer { get; private set; }

    public ObservableCollection<MonthPillViewModel> MonthPills { get; init; } = new();

    [ObservableProperty]
    private bool isShowPaymentField;

    [ObservableProperty]
    private bool isLoading;

    [NotifyPropertyChangedFor(nameof(FilteredTodoItems))]
    [ObservableProperty]
    private MonthPillViewModel selectedMonthPill;

    async partial void OnSelectedMonthPillChanged(MonthPillViewModel value)
    {
        if (value == null) { return; }
        foreach (var item in MonthPills)
        {
            item.IsSelected = false;
        }
        value.IsSelected = true;
        await RefreshSource();
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

    [ObservableProperty]
    private TodoListTabStatusBarViewModel statusBarData = new();

    public bool HasActiveTasks
    {
        get => Employeer.TodoItems?.Any(t => t.IsCompleted == false) ?? false;
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

    public EmployeerTabViewModel(IJobItemRepository jobItemRepository,
        IEmployeerPaymentRepository paymentRepository)
    {
        this.jobItemRepository = jobItemRepository;
        this.paymentRepository = paymentRepository;

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void SetEmployeer(EmployeerViewModel emp)
    {
        Employeer = emp;
        this.NewPayment = new EmployeerPaymentViewModel() { EmployeerId = emp.Id, EmployeerName = emp.Name };
        Employeer.TodoItems.CollectionChanged += async (o, e) => await RefreshSource();
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

        (await jobItemRepository.GetAllJobItems(Employeer.Id))
            .Select(i => i.ToViewModel())
            .ToList()
            .ForEach(i =>
            {
                Employeer.TodoItems.Add(i);
                i.Initialize();
            });

        (await paymentRepository.GetAllPaymentsForEmployeer(Employeer.Id))
            .Select(i => i.ToViewModel())
            .ToList()
            .ForEach(i => Employeer.Payments.Add(i));
        Employeer.Initialize();

        await App.Current.Dispatcher.InvokeAsync(() => LoadMonthPills());

        await RefreshSource();

        StatusBarData.SetSourceItems(FilteredTodoItems);

        WeakReferenceMessenger.Default.Send(new EmployeeTabLoadedMessage(this));

        IsLoading = false;
    }

    [RelayCommand]
    private void AddEmployeerPayment()
    {
        paymentRepository.AddPayment(NewPayment.ToEntity());
        Employeer.Payments.Insert(0, NewPayment);
        IsShowPaymentField = false;
        NewPayment = new EmployeerPaymentViewModel() { EmployeerId = Employeer.Id, EmployeerName = Employeer.Name };
    }

    private void LoadMonthPills()
    {
        var pillsData = Employeer.TodoItems.Select(i => i.StartDate)
            .Select(date => new { date.Year, date.Month })
            .GroupBy(i => $"{i.Year}-{i.Month}")
            .Select(i => i.First())
            .OrderByDescending(i => i.Year)
            .ThenByDescending(i => i.Month)
            .Take(MAX_PILLS_COUNT);

        foreach (var kvp in pillsData)
        {
            var pill = new MonthPillViewModel(this) { MonthNumber = kvp.Month, Year = kvp.Year };
            MonthPills.Add(pill);
        }

        SelectedMonthPill = MonthPills?.FirstOrDefault();
    }

    [RelayCommand]
    public void AddJobItem()
    {
        var item = new JobItemViewModel()
        {
            StartDate = DateTimeOffset.UtcNow,
            Title = "Новая задача",
            EmployeerId = Employeer.Id
        };
        item.Id = jobItemRepository.AddOrUpdateJobItem(item.ToEntity());
        Employeer.TodoItems.Add(item);
        item.Initialize();
    }

    [RelayCommand]
    private void DeleteSelectedJobItem(JobItemViewModel item)
    {
        if (item == null) { return; }

        var answer = MessageBox.Show($"Удалить выделенное задание ?\r\n'{item.Title}' от [{item.StartDate}]", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) { return; }

        jobItemRepository.DeleteJobItem(item.Id);
        Employeer.TodoItems.Remove(item);

        SelectedJobItem = null;
    }

    private async Task RefreshSource()
    {
        var source = Enumerable.Empty<JobItemViewModel>().ToList();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            throw new NotImplementedException();
        }
        else
        {
            var activePill = SelectedMonthPill ?? MonthPills.FirstOrDefault() ?? null;
            if (activePill != null)
            {
                var data = Employeer.TodoItems
                .Where(i => i.StartDate.Month == activePill.MonthNumber && i.StartDate.Year == activePill.Year)
                .OrderByDescending(i => i.IsCompleted ? 0 : 1)
                .ThenByDescending(i => i.StartDate)
                .ToList();

                source = data;
            }
        }

        FilteredTodoItems = source;
    }

    public void Receive(JobItemFieldUpdatedMessage message)
    {
        if (message.FieldName == nameof(message.Value.IsCompleted))
        {
            OnPropertyChanged(nameof(HasActiveTasks));
        }
    }

    public void Receive(MonthPillSelectionChangedMessage message)
    {
        if (message.Value.ParentEmployee == this)
        {
            SelectedMonthPill = message.Value;
        }

        StatusBarData.SetSourceItems(FilteredTodoItems);
    }
}