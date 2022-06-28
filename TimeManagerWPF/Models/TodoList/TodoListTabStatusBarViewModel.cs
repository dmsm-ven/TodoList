using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TodoList.WPF.ViewModels;

public class TodoListTabStatusBarViewModel : ViewModelBase
{
    private readonly IEnumerable<JobItemViewModel> jobItems;  
    public int ActiveTasks => jobItems?.Count(t => !t.IsCompleted) ?? 0;
    public string ActiveTasksMessage => $"Активные задачи: {ActiveTasks} из {jobItems.Count()}";

    public decimal TotalWorkCash => jobItems?.Where(t => t.IsCompleted).Sum(t => t.Price) ?? 0;
    public string TotalWorkCashMessage => $"Итого: {TotalWorkCash:C0}";

    public decimal AlreadyPayedTasksCash => jobItems?.Where(t => t.IsPayed && t.IsCompleted).Sum(t => t.Price) ?? 0;
    public string AlreadyPayedTasksCashMessage => $"Оплачено: {AlreadyPayedTasksCash:C0}";

    public decimal NotPayedTasksCash => TotalWorkCash - AlreadyPayedTasksCash;
    public string NotPayedTasksCashMessage => $"Не оплачено: {NotPayedTasksCash:C0}";

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
