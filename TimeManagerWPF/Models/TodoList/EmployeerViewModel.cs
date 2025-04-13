
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TodoList.WPF.Models.Messages;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models.TodoList;

public partial class EmployeerViewModel : ObservableRecipient,
    IRecipient<JobItemFieldUpdatedMessage>
{
    public DateTimeOffset Created { get; init; } = DateTimeOffset.UtcNow;
    public int Id { get; init; }

    [ObservableProperty]
    private string name;

    public int ActiveTasksCount => TodoItems.Count(t => !t.IsCompleted);

    [ObservableProperty]
    private ObservableCollection<EmployeerPaymentViewModel> payments = new();

    [ObservableProperty]
    private ObservableCollection<JobItemViewModel> todoItems = new();

    public EmployeerViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(JobItemFieldUpdatedMessage message)
    {
        if (message.FieldName == nameof(message.Value.IsCompleted))
        {
            OnPropertyChanged(nameof(ActiveTasksCount));
        }
    }
}
