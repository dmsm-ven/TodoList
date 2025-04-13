using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Linq;
using TodoList.WPF.Models.Messages;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models.TodoList;

public partial class TodoListTabStatusBarViewModel : ObservableRecipient,
    IRecipient<JobItemFieldUpdatedMessage>,
    IRecipient<MonthPillSelectionChangedMessage>
{
    private IEnumerable<JobItemViewModel> sourceItems;

    public int ActiveTasks => sourceItems?.Count(t => !t.IsCompleted) ?? 0;
    public string ActiveTasksMessage => $"Активные задачи: {ActiveTasks} из {sourceItems.Count()}";

    public decimal TotalWorkCash => sourceItems?.Where(t => t.IsCompleted).Sum(t => t.Price) ?? 0;
    public string TotalWorkCashMessage => $"Итого: {TotalWorkCash:C0}";

    public decimal AlreadyPayedTasksCash => sourceItems?.Where(t => t.IsPayed && t.IsCompleted).Sum(t => t.Price) ?? 0;
    public string AlreadyPayedTasksCashMessage => $"Оплачено: {AlreadyPayedTasksCash:C0}";

    public decimal NotPayedTasksCash => TotalWorkCash - AlreadyPayedTasksCash;
    public string NotPayedTasksCashMessage => $"Не оплачено: {NotPayedTasksCash:C0}";

    public TodoListTabStatusBarViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void SetSourceItems(IEnumerable<JobItemViewModel> newItemsSource)
    {
        sourceItems = newItemsSource;
    }

    public void Receive(JobItemFieldUpdatedMessage message)
    {
        RefreshProperties();
    }

    public void Receive(MonthPillSelectionChangedMessage message)
    {
        RefreshProperties();
    }

    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(ActiveTasks));
        OnPropertyChanged(nameof(ActiveTasksMessage));
        OnPropertyChanged(nameof(TotalWorkCash));
        OnPropertyChanged(nameof(TotalWorkCashMessage));
        OnPropertyChanged(nameof(AlreadyPayedTasksCash));
        OnPropertyChanged(nameof(AlreadyPayedTasksCashMessage));
        OnPropertyChanged(nameof(NotPayedTasksCash));
        OnPropertyChanged(nameof(NotPayedTasksCashMessage));
    }
}
