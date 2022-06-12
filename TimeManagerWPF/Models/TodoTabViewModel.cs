using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace TodoList.WPF.ViewModels;


public class TodoTabViewModel : ViewModelBase
{
    public string EmployeerName { get; set; }
    public bool HasActiveTask { get; set; }

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

    public List<JobItemViewModel> FilteredTodoItems
    {
        get => TodoItems
            .Where(i => i.StartDate.Month == SelectedMonthPill.MonthNumber && i.StartDate.Year == SelectedMonthPill.Year)
            .OrderByDescending(i => i.StartDate)
            .ToList();
    }
    public ObservableCollection<JobItemViewModel> TodoItems { get; set; }

    public ObservableCollection<MonthPillModel> MonthPills { get; set; }

    public ICommand AddNewJobItemCommand { get; }

    public TodoTabViewModel()
    {
        TodoItems = new ObservableCollection<JobItemViewModel>();
        MonthPills = new ObservableCollection<MonthPillModel>();
        AddNewJobItemCommand = new LambdaCommand(AddNewJobItem);     
        AddTestData();
        TodoItems.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(FilteredTodoItems));
        SelectedMonthPill = MonthPills.First();
        SelectedMonthPill.IsActive = true;
    }

    private void AddTestData()
    {
        TodoItems = new ObservableCollection<JobItemViewModel>();
        Enumerable.Range(1, 50).ToList().ForEach(i =>
        {
            var item = new JobItemViewModel()
            {
                Title = $"Задача #{i}",
                Description = $"Описание задачи #{i}",
                StartDate = DateTime.Now.AddDays(-i),
                EndDate = DateTime.Now.AddDays(i),
                IsCompleted = new Random().Next(0, 1) == 0 ? false : true,
                IsPayed = new Random().Next(0, 1) == 0 ? false : true,
                Price = new Random().Next(400, 3000),
                Screenshots = new List<string>((new Random().Next(-5, 2) > 0) ? new string[] { "screen.jpg" } : new string[] { string.Empty }),
                Website = "http://etk-komplekt.ru",
                Id = i
            };
            item.OnScreenshotsClicked += () => OpenScreenshotFolder(item);
            TodoItems.Add(item);
        });

        MonthPills = new ObservableCollection<MonthPillModel>();
        for (int month = DateTime.Now.Month; month >= DateTime.Now.Month - 3; month--)
        {
            var pill = new MonthPillModel(DateTime.Now.Year, month);
            MonthPills.Add(pill);
            pill.OnClicked += () =>
            {
                MonthPills.ToList().ForEach(i => i.IsActive = false);
                SelectedMonthPill = pill;
            };
        }

    }

    private void AddNewJobItem(object obj)
    {
        var item = new JobItemViewModel()
        {
            StartDate = DateTime.Now,
            Title = "Новая задача",
        };
        TodoItems.Add(item);
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
