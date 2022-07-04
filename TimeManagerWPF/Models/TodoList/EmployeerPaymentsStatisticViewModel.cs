using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models;

public class EmployeerPaymentsStatisticViewModel : ViewModelBase
{
    private readonly List<PropertyInfo> pi;

    public EmployeerViewModel Employeer { get; }

    public decimal? Month1Sum => GetStatisticForPeriod(1);
    public decimal? Month3Sum => GetStatisticForPeriod(3);
    public decimal? Month6Sum => GetStatisticForPeriod(6);
    public decimal? Month12Sum => GetStatisticForPeriod(12);

    public double? Month1Change => GetStatisticChangesPercentForPeriod(1);
    public double? Month3Change => GetStatisticChangesPercentForPeriod(3);
    public double? Month6Change => GetStatisticChangesPercentForPeriod(6);
    public double? Month12Change => GetStatisticChangesPercentForPeriod(12);

    public EmployeerPaymentsStatisticViewModel(EmployeerViewModel employeer)
    {
        Employeer = employeer;
        pi = this.GetType().GetProperties().ToList();
        Employeer.Payments.CollectionChanged += (o, e) =>
        {
            pi.ForEach(prop => RaisePropertyChanged(prop.Name));
        };
    }
    
    private decimal? GetStatisticForPeriod(int months)
    {
        if(Employeer.Payments.Count(p => p.TransferArrivalDate <= DateTime.Now.AddMonths(-months + 1)) == 0)
        {
            return null;
        }

        return Employeer?.Payments
            ?.Where(p => p.TransferArrivalDate >= DateTime.Now.AddMonths(-months))
            ?.Sum(t => t.Amount) ?? null;
    }

    public double? GetStatisticChangesPercentForPeriod(int months)
    {
        return null;
        //var currentMonthPayments = GetStatisticForPeriod(months);
        //if (currentMonthPayments == null)
        //{
        //    return null;
        //}
        //
        //var avgPerMonths = Employeer.Payments
        //    .GroupBy(p => $"{p.TransferArrivalDate.Year}-{p.TransferArrivalDate.Month}")
        //    .Average(g => g.Sum(p => p.Amount));
        //
        //if(avgPerMonths == decimal.Zero) { return null; }
        //
        //double changes = (double)currentMonthPayments.Value / (double)avgPerMonths;
        //
        //return changes;
    }
}
