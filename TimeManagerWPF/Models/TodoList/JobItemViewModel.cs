using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Diagnostics;
using TodoList.WPF.Models.Messages;

namespace TodoList.WPF.ViewModels;

public partial class JobItemViewModel : ObservableObject
{
    public int Id { get; set; }
    public int EmployeerId { get; set; }

    [ObservableProperty] private string title = string.Empty;

    [ObservableProperty] private string description = string.Empty;

    [ObservableProperty] private string website = string.Empty;

    [ObservableProperty] private decimal price;

    [ObservableProperty] private DateTimeOffset startDate;

    [ObservableProperty] private DateTimeOffset? endDate;

    [ObservableProperty] private bool isPayed;

    [ObservableProperty] private bool isCompleted;

    partial void OnIsCompletedChanged(bool newValue)
    {
        if (newValue)
        {
            EndDate = DateTimeOffset.Now;
        }
        else
        {
            EndDate = null;
        }
    }

    public int DaysAgo => (int)Math.Floor((DateTimeOffset.Now - StartDate).TotalDays);

    [RelayCommand]
    private void ShowChangeHistory()
    {
        WeakReferenceMessenger.Default.Send(new JobItemHistoryDisplayMessage(this));
    }

    [RelayCommand]
    private void OpenScreenshotsFolder()
    {
        WeakReferenceMessenger.Default.Send(new JobItemScreenshotShowMessage(this));
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