using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TodoList.WPF.Models;

public partial class EmployeerPaymentViewModel : ObservableObject
{
    public int Id { get; init; }
    public int EmployeerId { get; init; }
    public string EmployeerName { get; init; }

    [ObservableProperty]
    private DateTimeOffset transferArrivalDate = DateTimeOffset.Now;

    [ObservableProperty]
    private decimal amount;
}
