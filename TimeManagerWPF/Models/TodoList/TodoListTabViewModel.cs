using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models;
using TodoList.WPF.Views;

namespace TodoList.WPF.ViewModels;

public class TodoListTabViewModel : ViewModelBase
{
    private readonly IJobItemRepository jobItemRepository;
    private readonly IEmployeerPaymentRepository paymentRepository;
    private readonly IMapper mapper;

    private bool isShowPaymentField;
    public bool IsShowPaymentField 
    { 
        get => isShowPaymentField;
        set => Set(ref isShowPaymentField, value); 
    }

    private bool isLoading;
    public bool IsLoading 
    { 
        get => isLoading; 
        set => Set(ref isLoading, value); 
    }

    TodoListTabStatusBarViewModel statusBarData;  
    public TodoListTabStatusBarViewModel StatusBarData
    {
        get => statusBarData;
        set => Set(ref statusBarData, value);
    }
    
    public EmployeerViewModel Employeer { get; init; }
    EmployeerPaymentsStatisticViewModel paymentsStatistic;
    
    public EmployeerPaymentsStatisticViewModel PaymentsStatistic
    {
        get => paymentsStatistic;
        private set => Set(ref paymentsStatistic, value);
    }
    public ObservableCollection<MonthPillModel> MonthPills { get; init; }

    MonthPillModel selectedMonthPill;
    public MonthPillModel SelectedMonthPill
    {
        get => selectedMonthPill;
        set
        {
            if (Set(ref selectedMonthPill, value))
            {
                RaisePropertyChanged(nameof(FilteredTodoItems));
                StatusBarData = new TodoListTabStatusBarViewModel(FilteredTodoItems);
            }
        }
    }
    
    JobItemViewModel selectedJobItem;
    public JobItemViewModel SelectedJobItem
    {
        get => selectedJobItem;
        set => Set(ref selectedJobItem, value);
    }
    
    public List<JobItemViewModel> FilteredTodoItems
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                var activePill = SelectedMonthPill ?? MonthPills.FirstOrDefault() ?? null;
                if (activePill != null)
                {
                    var data = Employeer.TodoItems
                    .Where(i => i.StartDate.Month == activePill.MonthNumber && i.StartDate.Year == activePill.Year)
                    .OrderByDescending(i => i.IsCompleted ? 0 : 1)
                    .ThenByDescending(i => i.StartDate)
                    .ToList();

                    return data;
                }
            }
            else
            {
                return Employeer.TodoItems
                    .Where(item => item.HasText(SearchText))
                    .OrderByDescending(item => item.StartDate)
                    .ToList();
            }
            return new List<JobItemViewModel>();
        }
    }
    
    EmployeerPaymentViewModel newPayment;
    public EmployeerPaymentViewModel NewPayment
    {
        get => newPayment;
        set => Set(ref newPayment, value);
    }

    string searchText;
    public string SearchText
    {
        get => searchText;
        set
        {
            if(Set(ref searchText, value))
            {
                IsLoading = true;
                RaisePropertyChanged(nameof(FilteredTodoItems));
                IsLoading = false;
            }
        }
    }

    public bool HasActiveTasks
    {
        get => Employeer.TodoItems?.Any(t => t.IsCompleted == false) ?? false;
    }

    public ICollectionView SortedEmployeerPayments { get; }

    public IEnumerable<string> UniqueWebsites
    {
        get
        {
            if(Employeer.TodoItems.Count == 0)
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

    public ICommand AddJobCommand { get; }  
    public ICommand LoadedCommand { get; }
    public ICommand DeleteJobCommand { get; }
    public ICommand ShowPaymentFieldCommand { get; }
    public ICommand AddEmployeerPaymentCommand { get; }
   
    public TodoListTabViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
        AddJobCommand = new LambdaCommand(AddJobItem);
        DeleteJobCommand = new LambdaCommand(DeleteSelectedJobItem);
        ShowPaymentFieldCommand = new LambdaCommand(e => IsShowPaymentField = !IsShowPaymentField);
        AddEmployeerPaymentCommand = new LambdaCommand(AddEmployeerPayment, e => (NewPayment?.Amount ?? 0) != 0);
        MonthPills = new ObservableCollection<MonthPillModel>();
    }
   
    public TodoListTabViewModel(EmployeerViewModel employeer, IJobItemRepository jobItemRepository, IEmployeerPaymentRepository paymentRepository, IMapper mapper) : this()
    {
        this.jobItemRepository = jobItemRepository;
        this.paymentRepository = paymentRepository;
        this.mapper = mapper;
        Employeer = employeer;

        PaymentsStatistic = new EmployeerPaymentsStatisticViewModel(Employeer);
        NewPayment = new EmployeerPaymentViewModel() { EmployeerId = Employeer.Id };
        SortedEmployeerPayments = CollectionViewSource.GetDefaultView(Employeer.Payments);
        SortedEmployeerPayments.SortDescriptions.Add(new SortDescription(nameof(EmployeerPaymentViewModel.TransferArrivalDate), ListSortDirection.Descending));

        Employeer.TodoItems.CollectionChanged += TodoItems_CollectionChanged;
        Employeer.TodoItems.ToList().ForEach(item =>
        {
            item.OnShowHistoryClicked += Item_OnShowHistoryClicked;
            item.OnScreenshotsClicked += Item_OnScreenshotsClicked;
            item.PropertyChanged += Item_PropertyChanged;
        });
    }

    private void Loaded(object o)
    {
        if (Employeer == null) { return; }

        IsLoading = true;
        LoadMonthPills();
        IsLoading = false;
        SortedEmployeerPayments.Refresh();
    }
    
    private void AddEmployeerPayment(object obj)
    {
        paymentRepository.AddPayment(mapper.Map<EmployeerPaymentEntity>(NewPayment));
        Employeer.Payments.Insert(0, NewPayment);
        IsShowPaymentField = false;
        NewPayment = new EmployeerPaymentViewModel();
        SortedEmployeerPayments.Refresh();
    }
    
    private void LoadMonthPills()
    {
        var pillsData = Employeer.TodoItems.Select(i => i.StartDate)
            .Select(date => new { Year = date.Year, Month = date.Month })
            .GroupBy(i => $"{i.Year}-{i.Month}")
            .Select(i => i.First())
            .OrderByDescending(i => i.Year)
            .ThenByDescending(i => i.Month);

        MonthPills.Clear();

        foreach (var kvp in pillsData)
        {
            var pill = new MonthPillModel(kvp.Year, kvp.Month);
            MonthPills.Add(pill);
            pill.OnClicked += () =>
            {
                MonthPills.ToList().ForEach(i => i.IsActive = false);
                SelectedMonthPill = pill;
            };
        }

        SelectedMonthPill = MonthPills?.FirstOrDefault();
        if (SelectedMonthPill != null)
        {
            SelectedMonthPill.IsActive = true;
        }
    }
    
    public void AddJobItem(object o)
    {
        var item = new JobItemViewModel()
        {
            StartDate = DateTime.Now,
            Title = "Новая задача",
            EmployeerId = this.Employeer.Id
        };

        item.Id = jobItemRepository.AddOrUpdateJobItem(mapper.Map<JobItemEntity>(item));
        Employeer.TodoItems.Add(item);
    }
    
    internal void DeleteSelectedJobItem(object o)
    {
        if(SelectedJobItem == null) { return; }
        var answer = MessageBox.Show($"Удалить выделенное задание ?\r\n'{SelectedJobItem.Title}' от [{SelectedJobItem.StartDate}]", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) { return; }

        var temp = SelectedJobItem;
        Employeer.TodoItems.Remove(temp);
        jobItemRepository.DeleteJobItem(temp.Id);

        SelectedJobItem = null;
    }
    
    private void OpenScreenshotFolder(string job_item_id)
    {
        string folder = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),
            "screenshots",
            Employeer.Name,
            $"{SelectedMonthPill.Year}-{SelectedMonthPill.MonthName}",
            job_item_id);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        Process.Start("explorer.exe", folder);
    }

    private void Item_OnScreenshotsClicked(object? sender, EventArgs e)
    {
        OpenScreenshotFolder((sender as JobItemViewModel).Id.ToString());
    }

    private void Item_OnShowHistoryClicked(object? sender, EventArgs e)
    {
        int id = (sender as JobItemViewModel).Id;
        var window = new JobItemChangesHistoryWindow();
        window.DataContext = new JobItemChangesHistoryWindowViewModel(id, jobItemRepository, mapper);
        window.ShowDialog();
    }

    private void TodoItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RaisePropertyChanged(nameof(FilteredTodoItems));

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var newItem = ((IEnumerable<JobItemViewModel>)sender).Last();
            bool hasPill = Employeer.TodoItems.Any(i => i != newItem && i.StartDate.Year == newItem.StartDate.Year && i.StartDate.Month == newItem.StartDate.Month);
            if (!hasPill)
            {
                MonthPills.Insert(0, new MonthPillModel(newItem.StartDate));
                SelectedMonthPill = MonthPills[0];
            }

            newItem.OnScreenshotsClicked += Item_OnScreenshotsClicked;
            newItem.OnShowHistoryClicked += Item_OnShowHistoryClicked;
            newItem.PropertyChanged += Item_PropertyChanged;
        }
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        IsLoading = true;
        try
        {
            var item = mapper.Map<JobItemEntity>(sender);
            jobItemRepository.AddOrUpdateJobItem(item);

            string? newValue = sender.GetType()?.GetProperty(e.PropertyName).GetValue(sender)?.ToString() ?? string.Empty;

            jobItemRepository.AddHistoryChanges(item.Id, e.PropertyName, newValue);
            if (e.PropertyName == nameof(JobItemViewModel.IsCompleted))
            {
                RaisePropertyChanged(nameof(HasActiveTasks));
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}