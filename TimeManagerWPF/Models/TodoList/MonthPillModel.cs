using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;

namespace TodoList.WPF.Models.TodoList;

public record MonthPillSelectionChangedMessage(MonthPillViewModel Value);

public partial class MonthPillViewModel : ObservableObject
{
    public required int MonthNumber { get; init; }
    public required int Year { get; init; }

    public string MonthName => new DateTime(Year, MonthNumber, 1).ToString("MMMM");

    public string DisplayName
    {
        get
        {
            if (Year == DateTimeOffset.Now.Year)
            {
                return MonthName;
            }
            return $"{MonthName} | {Year}";
        }
    }

    [ObservableProperty]
    private bool isActive;

    [RelayCommand]
    private void MonthPillClick()
    {
        IsActive = true;
        WeakReferenceMessenger.Default.Send(new MonthPillSelectionChangedMessage(this));
    }
}
