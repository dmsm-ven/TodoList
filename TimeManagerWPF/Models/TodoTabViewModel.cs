using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess;

namespace TodoList.WPF.ViewModels;

public class TodoTabViewModel : ViewModelBase
{
    private readonly IJobItemRepository jobItemRepository;
    private readonly IMapper mapper;
    public bool IsLoaded { get; private set; }

    public string EmployeerName { get; set; }
    public int EmployeerId { get; set; }
    public bool HasActiveTask
    {
        get => TodoItems.Any(t => !t.IsCompleted && !t.IsPayed);
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
    public List<JobItemViewModel> FilteredTodoItems
    {
        get
        {
            var activePill = SelectedMonthPill ?? MonthPills.FirstOrDefault() ?? null;
            if (activePill != null)
            {
                var data = TodoItems
                .Where(i => i.StartDate.Month == activePill.MonthNumber && i.StartDate.Year == activePill.Year)
                .OrderByDescending(i => i.StartDate)
                .ToList();

                return data;
            }
            return new List<JobItemViewModel>();
        }
    }
    public ObservableCollection<JobItemViewModel> TodoItems { get; private set; }
    public ObservableCollection<MonthPillModel> MonthPills { get; private set; }

    public ICommand LoadedCommand { get; }

    public TodoTabViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
        TodoItems = new ObservableCollection<JobItemViewModel>();
        MonthPills = new ObservableCollection<MonthPillModel>();
        TodoItems.CollectionChanged += (o, e) =>
        {
            if (IsLoaded) { return; }
            RaisePropertyChanged(nameof(FilteredTodoItems));
            SelectedJobItem = null;
        };
    }

    public TodoTabViewModel(IJobItemRepository jobItemRepository, IMapper mapper) : this()
    {
        this.jobItemRepository = jobItemRepository;
        this.mapper = mapper;
    }

    private void Loaded(object obj)
    {
        LoadItems();
        LoadMonthPills();
        IsLoaded = true;
    }

    private void LoadItems()
    {
        jobItemRepository.GetAllJobItems(EmployeerId)
            .Select(i => mapper.Map<JobItemViewModel>(i))
            .ToList()
            .ForEach(i => {
                TodoItems.Add(i);
                AddItemEvents(i);
            });
    }

    private void LoadMonthPills()
    {
        var pillsData = TodoItems.Select(i => i.StartDate)
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

    private void AddItemEvents(JobItemViewModel item)
    {
        TodoItems.Add(item);

        item.OnScreenshotsClicked += () => OpenScreenshotFolder(item);
        item.PropertyChanged += (o, e) => SaveItemChanges(item);
    }

    private void SaveItemChanges(JobItemViewModel changedItem)
    {
        jobItemRepository.AddOrUpdateJobItem(mapper.Map<JobItemEntity>(changedItem));
    }
    
    public void AddJobItem()
    {
        var item = new JobItemViewModel()
        {
            StartDate = DateTime.Now,
            Title = "Новая задача",
            EmployeerId = this.EmployeerId
        };
        
        AddItemEvents(item);

        int max_id = jobItemRepository.AddOrUpdateJobItem(mapper.Map<JobItemEntity>(item));

        item.Id = max_id;
    }

    internal void DeleteJobItem()
    {
        if(SelectedJobItem != null)
        {
            var temp = SelectedJobItem;
            TodoItems.Remove(temp);
            jobItemRepository.DeleteJobItem(temp.Id);
        }
        SelectedJobItem = null;
    }

    private void OpenScreenshotFolder(JobItemViewModel item)
    {
        string folder = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),
            "screenshots",
            EmployeerName,
            $"{SelectedMonthPill.Year}-{SelectedMonthPill.MonthName}",
            item.Id.ToString());
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        Process.Start("explorer.exe", folder);
    }
}
