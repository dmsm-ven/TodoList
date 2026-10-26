using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using TodoListApp.Desktop.Infrastructure;
using TodoListApp.Desktop.Models.Messages;

namespace TodoListApp.Desktop.ViewModels;

public partial class JobItemViewModel : ObservableObject
{
    public int Id { get; set; }
    public int EmployeerId { get; set; }

    private bool isInitialized = false;

    // true, пока EndDate меняется автоматически из-за смены IsCompleted (чтобы не сохранять задачу дважды)
    private bool isSyncingEndDate = false;

    /// <summary>Свойства, изменение которых нужно сохранить на сервере (вычисляемые, как DaysAgo, не входят)</summary>
    private static readonly HashSet<string> persistedProperties = new()
    {
        nameof(Title), nameof(Description), nameof(Website), nameof(Price),
        nameof(StartDate), nameof(EndDate), nameof(IsPayed), nameof(IsCompleted)
    };

    /// <summary>Названия полей в истории изменений. Ключи - имена колонок БД (так их пишет API).</summary>
    public static readonly IReadOnlyDictionary<string, string> PropertyNameToTitle = new Dictionary<string, string>()
    {
        ["title"] = "Заголовок",
        ["description"] = "Описание",
        ["website"] = "Сайт",
        ["price"] = "Цена",
        ["start_date"] = "Дата создания",
        ["end_date"] = "Дата выполнения",
        ["is_payed"] = "Оплачено",
        ["is_completed"] = "Выполнено",
        ["employeer_id"] = "Работодатель"
    };

    [ObservableProperty] private string title = string.Empty;

    [ObservableProperty] private string description = string.Empty;

    [NotifyCanExecuteChangedFor(nameof(OpenWebsiteInBrowserCommand))]
    [ObservableProperty] private string website = string.Empty;

    [ObservableProperty] private decimal price;

    // Дата хранится как локальное время (DateTime.Kind = Local/Unspecified): DatePicker возвращает дату без пояса,
    // а при сохранении она переводится в UTC на сервере. При чтении нужно переводить обратно через LocalDateTime.
    [NotifyPropertyChangedFor(nameof(DaysAgo))]
    [ObservableProperty] private DateTime? startDate;

    partial void OnStartDateChanged(DateTime? oldValue, DateTime? newValue)
    {
        // Очистка DatePicker не допускается: дата создания обязательна (иначе NRE в фильтрах, пилюлях и при сохранении)
        if (newValue is null && oldValue is not null)
        {
            StartDate = oldValue;
        }
    }

    [ObservableProperty] private DateTimeOffset? endDate;

    [ObservableProperty] private bool isPayed;

    [ObservableProperty] private bool isCompleted;

    partial void OnIsCompletedChanged(bool value)
    {
        isSyncingEndDate = true;
        try
        {
            EndDate = value ? DateTimeOffset.UtcNow : null;
        }
        finally
        {
            isSyncingEndDate = false;
        }
    }

    /// <summary>Сколько календарных дней прошло с даты создания</summary>
    public int DaysAgo => StartDate is { } start ? (int)(DateTime.Today - start.Date).TotalDays : 0;

    public void Initialize()
    {
        if (isInitialized)
        {
            throw new InvalidOperationException("already initialized");
        }

        isInitialized = true;

        PropertyChanged += (s, e) =>
        {
            if (isSyncingEndDate || e.PropertyName is null || !persistedProperties.Contains(e.PropertyName))
            {
                return;
            }
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

    private bool CanOpenWebsite() => !string.IsNullOrWhiteSpace(Website);

    [RelayCommand(CanExecute = nameof(CanOpenWebsite))]
    private void OpenWebsiteInBrowser()
    {
        var address = Website.Trim();
        if (!address.Contains("://"))
        {
            address = "https://" + address;
        }

        // Открываем только http/https: UseShellExecute запустил бы и file://, и любое другое приложение
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            MessageBox.Show($"Некорректный адрес сайта: '{Website}'", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = uri.AbsoluteUri,
            });
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось открыть сайт в браузере");
        }
    }
}
