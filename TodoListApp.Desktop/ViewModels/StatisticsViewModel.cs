using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TodoListApp.Desktop.Infrastructure;
using TodoListApp.Desktop.Models;
using TodoListApp.Desktop.Models.TodoList;

namespace TodoListApp.Desktop.ViewModels;

public record MonthStatisticsRow(string Month, int CompletedCount, decimal Total, decimal Paid, decimal NotPaid);

/// <summary>Строка динамики: сумма выполненных задач за месяц и изменение к прошлому месяцу / тому же месяцу прошлого года.</summary>
public record MonthDynamicsRow(string Month, int CompletedCount, decimal Total, string ChangeToPrevious, string ChangeToLastYear, string ShortMonth);

/// <summary>Строка сравнения работодателей.</summary>
public record EmployeerComparisonRow(string Name, int CompletedCount, decimal Total, double SharePercent, decimal AveragePrice, string AverageDays);

/// <summary>Статистика по выполненным задачам и оплатам выбранного работодателя.</summary>
public partial class StatisticsViewModel : ObservableObject
{
    public IReadOnlyList<EmployeerTabViewModel> Employeers { get; }

    [ObservableProperty]
    private EmployeerTabViewModel? selectedTab;

    [ObservableProperty]
    private int completedCount;

    [ObservableProperty]
    private decimal totalSum;

    [ObservableProperty]
    private decimal paidSum;

    [ObservableProperty]
    private decimal notPaidSum;

    [ObservableProperty]
    private EmployeerPaymentsStatisticModel? paymentsStatistic;

    public ObservableCollection<MonthStatisticsRow> Months { get; } = new();

    public ObservableCollection<EmployeerPaymentViewModel> Payments { get; } = new();

    /// <summary>Динамика выбранного работодателя за последние 12 месяцев (от нового к старому).</summary>
    public ObservableCollection<MonthDynamicsRow> Dynamics { get; } = new();

    /// <summary>Сравнение всех работодателей по выполненным задачам.</summary>
    public ObservableCollection<EmployeerComparisonRow> Comparison { get; } = new();

    public StatisticsViewModel(IEnumerable<EmployeerTabViewModel> tabs, EmployeerTabViewModel? initialTab)
    {
        Employeers = tabs.ToList();
        SelectedTab = initialTab ?? Employeers.FirstOrDefault();

        _ = LoadAllAndCompareAsync();
    }

    private async System.Threading.Tasks.Task LoadAllAndCompareAsync()
    {
        try
        {
            // Для сравнения нужны данные всех работодателей: догружаем те, что еще не загружены
            foreach (var tab in Employeers)
            {
                await tab.LoadedCommand.ExecuteAsync(null);
            }
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось загрузить данные для сравнения");
        }

        RecalculateComparison();
        RecalculateDynamics();
    }

    private void RecalculateComparison()
    {
        Comparison.Clear();

        var stats = Employeers
            .Select(tab =>
            {
                var completed = tab.Employeer.TodoItems.Where(t => t.IsCompleted).ToList();
                var durations = completed
                    .Where(t => t.EndDate.HasValue && t.StartDate.HasValue)
                    .Select(t => (t.EndDate!.Value.LocalDateTime - t.StartDate!.Value).TotalDays)
                    .Where(d => d >= 0)
                    .ToList();

                return new
                {
                    tab.Employeer.Name,
                    Count = completed.Count,
                    Total = completed.Sum(t => t.Price),
                    AverageDays = durations.Count > 0 ? durations.Average().ToString("F1") + " дн." : "—"
                };
            })
            .ToList();

        var grandTotal = stats.Sum(s => s.Total);

        foreach (var s in stats.OrderByDescending(s => s.Total))
        {
            Comparison.Add(new EmployeerComparisonRow(
                s.Name,
                s.Count,
                s.Total,
                grandTotal > 0 ? (double)(s.Total / grandTotal * 100m) : 0,
                s.Count > 0 ? s.Total / s.Count : 0,
                s.AverageDays));
        }
    }

    private void RecalculateDynamics()
    {
        Dynamics.Clear();

        var employeer = SelectedTab?.Employeer;
        if (employeer == null) { return; }

        var totals = employeer.TodoItems
            .Where(t => t.IsCompleted && t.StartDate.HasValue)
            .GroupBy(t => (t.StartDate!.Value.Year, t.StartDate!.Value.Month))
            .ToDictionary(g => g.Key, g => (Count: g.Count(), Total: g.Sum(t => t.Price)));

        (int Count, decimal Total) Get(DateTime month) =>
            totals.TryGetValue((month.Year, month.Month), out var v) ? v : (0, 0m);

        var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var months = Enumerable.Range(0, 12).Select(i => first.AddMonths(-i)).ToList();

        foreach (var month in months)
        {
            var current = Get(month);
            Dynamics.Add(new MonthDynamicsRow(
                month.ToString("MMMM yyyy"),
                current.Count,
                current.Total,
                FormatChange(current.Total, Get(month.AddMonths(-1)).Total),
                FormatChange(current.Total, Get(month.AddYears(-1)).Total),
                month.ToString("MMM yy")));
        }
    }

    private static string FormatChange(decimal current, decimal previous)
    {
        if (previous == 0) { return "—"; }
        var change = (current - previous) / previous;
        return change.ToString("+0%;-0%;0%");
    }

    async partial void OnSelectedTabChanged(EmployeerTabViewModel? value)
    {
        if (value == null)
        {
            Recalculate();
            return;
        }

        try
        {
            // Если данные вкладки еще не загружены (фоновая подгрузка не закончилась), догружаем; для загруженной вкладки ничего не произойдет
            await value.LoadedCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            UiErrors.Show(ex, "Не удалось загрузить данные работодателя");
        }

        if (SelectedTab == value)
        {
            Recalculate();
        }
    }

    private void Recalculate()
    {
        Months.Clear();
        Payments.Clear();

        var employeer = SelectedTab?.Employeer;
        if (employeer == null)
        {
            CompletedCount = 0;
            TotalSum = PaidSum = NotPaidSum = 0;
            PaymentsStatistic = null;
            Dynamics.Clear();
            return;
        }

        var completed = employeer.TodoItems.Where(t => t.IsCompleted).ToList();

        CompletedCount = completed.Count;
        TotalSum = completed.Sum(t => t.Price);
        PaidSum = completed.Where(t => t.IsPayed).Sum(t => t.Price);
        NotPaidSum = TotalSum - PaidSum;

        // Месяц считается по дате создания задачи - так же, как плитки месяцев на главном окне
        var rows = completed
            .Where(t => t.StartDate.HasValue)
            .GroupBy(t => new { t.StartDate!.Value.Year, t.StartDate!.Value.Month })
            .OrderByDescending(g => g.Key.Year)
            .ThenByDescending(g => g.Key.Month)
            .Select(g => new MonthStatisticsRow(
                new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                g.Count(),
                g.Sum(t => t.Price),
                g.Where(t => t.IsPayed).Sum(t => t.Price),
                g.Where(t => !t.IsPayed).Sum(t => t.Price)));

        foreach (var row in rows)
        {
            Months.Add(row);
        }

        foreach (var payment in employeer.Payments.OrderByDescending(p => p.TransferArrivalDate))
        {
            Payments.Add(payment);
        }

        PaymentsStatistic = employeer.PaymentsStatistic;

        RecalculateDynamics();
    }
}
