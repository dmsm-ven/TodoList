using System;
using System.Collections.ObjectModel;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models;

public class EmployeerViewModel : ViewModelBase
{
    public int Id { get; init; }

    string name;
    public string Name
    {
        get => name;
        set => Set(ref name, value);
    }
    public DateTime Created { get; set; } = DateTime.Now;

    ObservableCollection<EmployeerPaymentViewModel> payments;
    public ObservableCollection<EmployeerPaymentViewModel> Payments
    {
        get => payments;
        set => Set(ref payments, value);
    }

    ObservableCollection<JobItemViewModel> todoItems;
    public ObservableCollection<JobItemViewModel> TodoItems
    {
        get => todoItems;
        set => Set(ref todoItems, value);
    }
}
