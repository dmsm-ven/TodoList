using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using TodoListApp.Desktop.Models.Messages;

namespace TodoListApp.Desktop.Models.TodoList;

public partial class MonthPillViewModel : ObservableObject
{
    public EmployeerTabViewModel ParentEmployee { get; }

    public required int MonthNumber { get; init; }
    public required int Year { get; init; }

    [ObservableProperty]
    private bool isSelected = false;

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

    public MonthPillViewModel(EmployeerTabViewModel parent)
    {
        ParentEmployee = parent;
    }

    [RelayCommand]
    private void MonthPillClick()
    {
        WeakReferenceMessenger.Default.Send(new MonthPillSelectionChangedMessage(this));
    }
}
