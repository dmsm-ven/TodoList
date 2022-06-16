using System;
using System.Collections.ObjectModel;
using TodoList.WPF.DataAccess;
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
    public ObservableCollection<EmployeerPaymentViewModel> Payments { get; init; }

    public ObservableCollection<JobItemViewModel> TodoItems { get; init; }
}
