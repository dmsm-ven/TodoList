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

    public StatisticsViewModel(IEnumerable<EmployeerTabViewModel> tabs, EmployeerTabViewModel? initialTab)
    {
        Employeers = tabs.ToList();
        SelectedTab = initialTab ?? Employeers.FirstOrDefault();
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
    }
}
