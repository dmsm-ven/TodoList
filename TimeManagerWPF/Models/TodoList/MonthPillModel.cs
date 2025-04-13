using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using TodoList.WPF.Models.Messages;

namespace TodoList.WPF.Models.TodoList;

public partial class MonthPillViewModel : ObservableObject
{
    private readonly EmployeerTabViewModel parent;

    public required int MonthNumber { get; init; }
    public required int Year { get; init; }

    public string MonthName => new DateTime(Year, MonthNumber, 1).ToString("MMMM");

    public string DisplayName
    {
        get
        {
            if (Year == DateTimeOffset.UtcNow.Year)
            {
                return MonthName;
            }
            return $"{MonthName} | {Year}";
        }
    }

    [ObservableProperty]
    private bool isActive;

    public MonthPillViewModel(EmployeerTabViewModel parent)
    {
        this.parent = parent;
    }

    [RelayCommand]
    private void MonthPillClick()
    {
        IsActive = true;
        WeakReferenceMessenger.Default.Send(new MonthPillSelectionChangedMessage(parent));
    }
}
