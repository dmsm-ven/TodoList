using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models;

public class EmployeerPaymentsStatisticViewModel : ViewModelBase
{
    public EmployeerViewModel Employeer { get; }

    public decimal Month1Sum => GetStatisticForPeriod(1);
    public decimal Month3Sum => GetStatisticForPeriod(3);
    public decimal Month6Sum => GetStatisticForPeriod(6);
    public decimal Month12Sum => GetStatisticForPeriod(12);

    public EmployeerPaymentsStatisticViewModel(EmployeerViewModel employeer)
    {
        Employeer = employeer;
        Employeer.Payments.CollectionChanged += (o, e) =>
        {
            RaisePropertyChanged(nameof(Month1Sum));
            RaisePropertyChanged(nameof(Month3Sum));
            RaisePropertyChanged(nameof(Month6Sum));
            RaisePropertyChanged(nameof(Month12Sum));
        };
    }
    
    private decimal GetStatisticForPeriod(int months)
    {
        return Employeer?.Payments
            ?.Where(p => p.TransferArrivalDate >= DateTime.Now.AddMonths(-months))
            ?.Sum(t => t.Amount) ?? 0;
    }
}
