using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels;

public class TodoTabViewModel : ViewModelBase
{
    private readonly IJobItemRepository jobItemRepository;
    private readonly IEmployeerPaymentRepository paymentRepository;
    private readonly IMapper mapper;

    private bool isShowPaymentField;
    public bool IsShowPaymentField { get => isShowPaymentField; set => Set(ref isShowPaymentField, value); }

    private bool isLoading;
    public bool IsLoading { get => isLoading; set => Set(ref isLoading, value); }

    EmployeerViewModel employeer;
    public EmployeerViewModel Employeer
    {
        get => employeer;
        set
        {
            if(Set(ref employeer, value))
            {
                PaymentsStatistic = new EmployeerPaymentsStatisticViewModel(value);
            }
        }
    }
    public EmployeerPaymentsStatisticViewModel PaymentsStatistic { get; private set; } 
    public bool HasActiveTasks
    {
        get => Employeer.TodoItems?.Any(t => t.IsCompleted == false) ?? false;
    }
    MonthPillModel selectedMonthPill;
    public MonthPillModel SelectedMonthPill
    {
        get => selectedMonthPill;
        set
        {
            if (Set(ref selectedMonthPill, value))
            {
                RaisePropertyChanged(nameof(FilteredTodoItems));
            }
        }
    }
    JobItemViewModel selectedJobItem;
    public JobItemViewModel SelectedJobItem
    {
        get => selectedJobItem;
        set => Set(ref selectedJobItem, value);
    }
    public IEnumerable<JobItemViewModel> FilteredTodoItems
    {
        get
        {
            var activePill = SelectedMonthPill ?? MonthPills.FirstOrDefault() ?? null;
            if (activePill != null)
            {
                var data = Employeer.TodoItems
                .Where(i => i.StartDate.Month == activePill.MonthNumber && i.StartDate.Year == activePill.Year)
                .OrderByDescending(i => i.StartDate);

                return data;
            }
            return new List<JobItemViewModel>();
        }
    }
    public ObservableCollection<MonthPillModel> MonthPills { get; private set; }
    int paymentAmount;
    public int PaymentAmount
    {
        get => paymentAmount;
        set => Set(ref paymentAmount, value);
    }
    public ICommand AddJobCommand { get; }  
    public ICommand LoadedCommand { get; }
    public ICommand DeleteJobCommand { get; }
    public ICommand ShowPaymentFieldCommand { get; }
    public ICommand AddEmployeerPaymentCommand { get; }
    public TodoTabViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
        AddJobCommand = new LambdaCommand(AddJobItem);
        DeleteJobCommand = new LambdaCommand(DeleteSelectedJobItem);
        ShowPaymentFieldCommand = new LambdaCommand(e => IsShowPaymentField = !IsShowPaymentField);
        AddEmployeerPaymentCommand = new LambdaCommand(AddEmployeerPayment, e => PaymentAmount != 0);
        MonthPills = new ObservableCollection<MonthPillModel>();
    }
    public TodoTabViewModel(IJobItemRepository jobItemRepository, IEmployeerPaymentRepository paymentRepository, IMapper mapper) : this()
    {
        this.jobItemRepository = jobItemRepository;
        this.paymentRepository = paymentRepository;
        this.mapper = mapper;
    }
    private async void Loaded(object o)
    {
        if (Employeer == null) { return; }

        IsLoading = true;
        await Task.Delay(TimeSpan.FromSeconds(0.25));
        LoadItems();
        LoadMonthPills();
        IsLoading = false;
    }
    private void AddEmployeerPayment(object obj)
    {
        var item = new EmployeerPaymentEntity() { EmployeerId = Employeer.Id, Amount = PaymentAmount, TransferArrivalDate = DateTime.Now };
        paymentRepository.AddPayment(item);
        Employeer.Payments.Insert(0, mapper.Map<EmployeerPaymentViewModel>(item));
        IsShowPaymentField = false;
    }
    private void LoadItems()
    {
        
        Employeer.TodoItems?.ToList().ForEach(i => i.PropertyChanged -= Item_PropertyChanged);
        Employeer.TodoItems = new ObservableCollection<JobItemViewModel>();
        jobItemRepository.GetAllJobItems(Employeer.Id)
            .Select(i => mapper.Map<JobItemViewModel>(i))
            .ToList()
            .ForEach(i => {
                AddItemAndEvents(i);
            });
    }
    private void LoadMonthPills()
    {
        var pillsData = Employeer.TodoItems.Select(i => i.StartDate)
            .Select(date => new { Year = date.Year, Month = date.Month })
            .Distinct()
            .OrderByDescending(i => i.Year)
            .ThenByDescending(i => i.Month)
            .ToDictionary(i => i.Year, i => i.Month);

        MonthPills.Clear();

        foreach (var kvp in pillsData)
        {
            var pill = new MonthPillModel(kvp.Key, kvp.Value);
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
    private void AddItemAndEvents(JobItemViewModel item)
    {
        Employeer.TodoItems.Add(item);
        item.OnScreenshotsClicked += () => OpenScreenshotFolder(item);
        item.PropertyChanged += Item_PropertyChanged;
        
    }
    private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        IsLoading = true;
        try
        {
            jobItemRepository.AddOrUpdateJobItem(mapper.Map<JobItemEntity>(sender));
            if(e.PropertyName == nameof(JobItemViewModel.IsCompleted))
            {
                RaisePropertyChanged(nameof(HasActiveTasks));
            }
        }
        finally
        {
            IsLoading = false;
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

        AddItemAndEvents(item);

        int max_id = jobItemRepository.AddOrUpdateJobItem(mapper.Map<JobItemEntity>(item));

        item.Id = max_id;
    }
    internal void DeleteSelectedJobItem(object o)
    {
        var answer = MessageBox.Show("Удалить выделенное задание ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) { return; }

        var temp = SelectedJobItem;
        Employeer.TodoItems.Remove(temp);
        jobItemRepository.DeleteJobItem(temp.Id);

        SelectedJobItem = null;
    }
    private void OpenScreenshotFolder(JobItemViewModel item)
    {
        string folder = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),
            "screenshots",
            Employeer.Name,
            $"{SelectedMonthPill.Year}-{SelectedMonthPill.MonthName}",
            item.Id.ToString());
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        Process.Start("explorer.exe", folder);
    }
}
