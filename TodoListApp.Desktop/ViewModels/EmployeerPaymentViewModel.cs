using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TodoListApp.Desktop.Models;

public partial class EmployeerPaymentViewModel : ObservableObject
{
    public int Id { get; init; }
    public int EmployeerId { get; init; }
    public string EmployeerName { get; init; }

    [ObservableProperty]
    private DateTime transferArrivalDate = DateTime.Now;

    [ObservableProperty]
    private decimal amount;
}
