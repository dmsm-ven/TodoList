using System;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models;

public class EmployeerPaymentViewModel : ViewModelBase
{
    public int Id { get; init; }
    public int EmployeerId { get; init; }
    public string EmployeerName { get; init; }

    DateTime transferArrivalDate = DateTime.Now;
    public DateTime TransferArrivalDate
    {
        get => transferArrivalDate;
        set => Set(ref transferArrivalDate, value);
    }

    decimal amount;
    public decimal Amount
    {
        get => amount;
        set => Set(ref amount, value);
    }
}
