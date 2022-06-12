using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace TodoList.WPF.ViewModels;

public class JobItemViewModel : ViewModelBase
{
    public event Action OnScreenshotsClicked;
    public int Id { get; set; }

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
    public bool IsPayed { get; set; }

    decimal price;
    public decimal Price
    {
        get => price;
        set => Set(ref price, value);
    }
    public DateTime StartDate { get; set; }

    DateTime? endDate;
    public DateTime? EndDate
    {
        get => endDate;
        set => Set(ref endDate, value);
    }
    public List<string> Screenshots { get; set; } = new List<string>();
    public ICommand OpenScreenshotsFolderCommand { get; }
    public bool HasScreenshots => Screenshots.Any(s => !string.IsNullOrWhiteSpace(s));
    public JobItemViewModel()
    {
        OpenScreenshotsFolderCommand = new LambdaCommand(
            e => OnScreenshotsClicked?.Invoke(), 
            e => HasScreenshots);
    }
}