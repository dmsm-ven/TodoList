using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TodoListApp.Desktop.Models.Messages;

namespace TodoListApp.Desktop.ViewModels;

public partial class JobItemViewModel : ObservableObject
{
    public int Id { get; set; }
    public int EmployeerId { get; set; }

    private bool isInitialized = false;

    public IReadOnlyDictionary<string, string> PropertyNameToTitle = new Dictionary<string, string>()
    {
        ["Title"] = "Заголовок",
        ["Description"] = "Описание",
        ["Website"] = "Сайт",
        ["Price"] = "Цена",
        ["StartDate"] = "Дата создания",
        ["EndDate"] = "Дата выполения",
        ["IsPayed"] = "Оплачено",
        ["IsCompleted"] = "Выполнено"
    };

    [ObservableProperty] private string title = string.Empty;

    [ObservableProperty] private string description = string.Empty;

    [ObservableProperty] private string website = string.Empty;

    [ObservableProperty] private decimal price;

    //TODO: пофиксить что при выборе в DateTimePicker берется UTC время, т.е. получается на 1 день меньше для +03 00
    [ObservableProperty] private DateTime? startDate;

    [ObservableProperty] private DateTimeOffset? endDate;

    [ObservableProperty] private bool isPayed;

    [ObservableProperty] private bool isCompleted;

    partial void OnIsCompletedChanged(bool newValue)
    {
        if (newValue)
        {
            EndDate = DateTimeOffset.UtcNow;
        }
        else
        {
            EndDate = null;
        }
    }

    public int DaysAgo => (int)Math.Floor((DateTimeOffset.UtcNow - StartDate.Value).TotalDays);

    public void Initialize()
    {
        if (isInitialized)
        {
            throw new InvalidOperationException("already initialized");
        }

        isInitialized = true;

        PropertyChanged += (s, e) =>
        {
            WeakReferenceMessenger.Default.Send(new JobItemFieldUpdatedMessage(this));
        };
    }

    [RelayCommand]
    private void ShowChangeHistory()
    {
        WeakReferenceMessenger.Default.Send(new JobItemHistoryDisplayMessage(this));
    }

    [RelayCommand]
    private void OpenScreenshotsFolder()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void OpenWebsiteInBrowser()
    {
        var myProcess = new ProcessStartInfo()
        {
            UseShellExecute = true,
            FileName = Website,
        };
        Process.Start(myProcess);
    }
}