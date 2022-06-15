using System;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models;

public class EmployeerViewModel : ViewModelBase
{
    public int Id { get; }

    string name;
    public string Name
    {
        get => name;
        set => Set(ref name, value);
    }
    public DateTime Created { get; set; } = DateTime.Now;


}
