using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TodoList.WPF.Models.Messages;

namespace TodoList.WPF.ViewModels;

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

    [ObservableProperty] private DateTimeOffset startDate;

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

    public int DaysAgo => (int)Math.Floor((DateTimeOffset.UtcNow - StartDate).TotalDays);

    public void Initialize()
    {
        isInitialized = true;

        PropertyChanged += (s, e) =>
        {
            if (!isInitialized) { return; }

            var pi = this.GetType().GetProperty(e.PropertyName);

            string title = PropertyNameToTitle[e.PropertyName];
            string value = string.Empty;

            if (pi.PropertyType == typeof(bool))
            {
                value = (bool)pi.GetValue(this) ? "Да" : "Нет";
            }
            else
            {
                value = pi.GetValue(this).ToString();
            }

            WeakReferenceMessenger.Default.Send(new JobItemFieldUpdatedMessage(this, title, value));
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