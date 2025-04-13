using System;
using System.Linq;

namespace TodoList.WPF.Models.TodoList;

public class EmployeerPaymentsStatisticModel
{
    public EmployeerViewModel Employeer { get; }

    public decimal? Month1Sum => GetStatisticForPeriod(1);
    public decimal? Month3Sum => GetStatisticForPeriod(3);
    public decimal? Month6Sum => GetStatisticForPeriod(6);
    public decimal? Month12Sum => GetStatisticForPeriod(12);

    public EmployeerPaymentsStatisticModel(EmployeerViewModel employeer)
    {
        Employeer = employeer;
    }

    private decimal? GetStatisticForPeriod(int months)
    {
        if (Employeer.Payments.Count(p => p.TransferArrivalDate <= DateTimeOffset.UtcNow.AddMonths(-months + 1)) == 0)
        {
            return null;
        }

        return Employeer?.Payments
            ?.Where(p => p.TransferArrivalDate >= DateTimeOffset.UtcNow.AddMonths(-months))
            ?.Sum(t => t.Amount) ?? null;
    }
}
