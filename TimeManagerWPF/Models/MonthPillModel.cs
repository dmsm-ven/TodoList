using System;
using System.Windows.Input;

namespace TodoList.WPF.ViewModels;

public class MonthPillModel : ViewModelBase
{
    public event Action OnClicked;
    public int MonthNumber { get;  }
    public int Year { get; }
    public string MonthName => new DateTime(Year, MonthNumber, 1).ToString("MMMM");
    public string DisplayName
    {
        get
        {
            if(Year == DateTime.Now.Year)
            {
                return MonthName;
            }
            return $"{MonthName} | {Year}";
        }
    }
    
    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set => Set(ref isActive, value);
    }
    public ICommand MonthPillClickCommand { get; }
    public MonthPillModel(int year, int month)
    {
        Year = year;
        MonthNumber = month;
        MonthPillClickCommand = new LambdaCommand(e =>
        {
            OnClicked?.Invoke();
            IsActive = true;
        }, e => true);
    }
}
