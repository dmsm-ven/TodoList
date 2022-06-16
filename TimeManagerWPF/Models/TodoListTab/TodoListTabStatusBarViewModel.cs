using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TodoList.WPF.ViewModels;

public class TodoListTabStatusBarViewModel : ViewModelBase
{
    private readonly IEnumerable<JobItemViewModel> jobItems;  
    public int ActiveTasks => jobItems?.Count(t => !t.IsCompleted) ?? 0;
    public string ActiveTasksMessage => $"Активных задач: {ActiveTasks}";
    public decimal TotalWorkCash => jobItems?.Sum(t => t.Price) ?? 0;
    public string TotalWorkCashMessage => $"Всего задач на сумму: {TotalWorkCash:C0}";
    public decimal AlreadyPayedTasksCash => jobItems?.Where(t => t.IsPayed).Sum(t => t.Price) ?? 0;
    public string AlreadyPayedTasksCashMessage => $"Из них оплачено: {AlreadyPayedTasksCash:C0}";
    public decimal NotPayedTasksCash => jobItems?.Where(t => !t.IsPayed).Sum(t => t.Price) ?? 0;
    public string NotPayedTasksCashMessage => $"Должны оплатить еще: {NotPayedTasksCash:C0}";

    public TodoListTabStatusBarViewModel()
    {

    }
    public TodoListTabStatusBarViewModel(IEnumerable<JobItemViewModel> jobItems) : this()
    {
        this.jobItems = jobItems;
        jobItems.ToList().ForEach(i => i.PropertyChanged += (o, e) =>
        {
            foreach (var pi in this.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                RaisePropertyChanged(pi.Name);
            }
        });
    }
}
