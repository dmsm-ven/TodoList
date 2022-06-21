using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace TodoList.WPF.ViewModels;

public class JobItemViewModel : ViewModelBase
{
    public event Action OnScreenshotsClicked;
    public int Id { get; set; }

    public int EmployeerId { get; set; }

    string title;
    public string Title
    {
        get => title;
        set => Set(ref title, value);
    }
    
    string description;
    public string Description
    {
        get => description;
        set => Set(ref description, value);
    }
    
    string website;
    public string Website
    {
        get => website;
        set => Set(ref website, value);
    }

    bool isCompleted;
    public bool IsCompleted
    {
        get => isCompleted;
        set
        {
            if(Set(ref isCompleted, value) && value)
            {
                EndDate = DateTime.Now;
            }
            else
            {
                EndDate = null;
            }
        }
    }

    bool isPayed;
    public bool IsPayed
    {
        get => isPayed;
        set => Set(ref isPayed, value);
    }

    decimal price;
    public decimal Price
    {
        get => price;
        set => Set(ref price, value);
    }

    DateTime startDate;
    public DateTime StartDate
    {
        get => startDate;
        set => Set(ref startDate, value);
    }

    DateTime? endDate;
    public DateTime? EndDate
    {
        get => endDate;
        set => Set(ref endDate, value);
    }

    public int DaysAgo => (int)Math.Floor((DateTime.Now - StartDate).TotalDays);

    public ICommand OpenScreenshotsFolderCommand { get; }
    public bool HasScreenshots => true;
    public JobItemViewModel()
    {
        OpenScreenshotsFolderCommand = new LambdaCommand(
            e => OnScreenshotsClicked?.Invoke(), 
            e => HasScreenshots);
    }
}