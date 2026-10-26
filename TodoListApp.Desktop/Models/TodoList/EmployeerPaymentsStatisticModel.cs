using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoListApp.Desktop.Models.TodoList;

public class EmployeerPaymentsStatisticModel : ObservableObject
{
    private IEnumerable<EmployeerPaymentViewModel>? data = new List<EmployeerPaymentViewModel>();

    public decimal? Month1Sum => GetStatisticForPeriod(1);
    public decimal? Month3Sum => GetStatisticForPeriod(3);
    public decimal? Month6Sum => GetStatisticForPeriod(6);
    public decimal? Month12Sum => GetStatisticForPeriod(12);

    public void SetSource(IEnumerable<EmployeerPaymentViewModel> data)
    {
        this.data = data;

        OnPropertyChanged(nameof(Month1Sum));
        OnPropertyChanged(nameof(Month3Sum));
        OnPropertyChanged(nameof(Month6Sum));
        OnPropertyChanged(nameof(Month12Sum));
    }

    private decimal? GetStatisticForPeriod(int months)
    {
        if ((data?.Count(p => p.TransferArrivalDate <= DateTimeOffset.UtcNow.AddMonths(-months + 1)) ?? 0) == 0)
        {
            return null;
        }

        return data
            ?.Where(p => p.TransferArrivalDate >= DateTimeOffset.UtcNow.AddMonths(-months))
            ?.Sum(t => t.Amount) ?? null;
    }
}
