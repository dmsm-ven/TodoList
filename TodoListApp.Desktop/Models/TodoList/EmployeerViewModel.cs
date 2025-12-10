
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TodoList.WPF.Models.Messages;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Models.TodoList;

public partial class EmployeerViewModel : ObservableRecipient,
    IRecipient<JobItemFieldUpdatedMessage>
{
    private bool isInitialized = false;

    public DateTimeOffset Created { get; init; } = DateTimeOffset.UtcNow;
    public int Id { get; init; }

    [ObservableProperty]
    private string name;

    public int ActiveTasksCount => TodoItems.Count(t => !t.IsCompleted);

    [ObservableProperty]
    private ObservableCollection<EmployeerPaymentViewModel> payments = new();

    public IEnumerable<EmployeerPaymentViewModel> SortedPaymenets =>
        Payments
        .OrderByDescending(x => x.TransferArrivalDate)
        .Take(10)
        .ToArray();

    [ObservableProperty]
    private ObservableCollection<JobItemViewModel> todoItems = new();

    [ObservableProperty]
    private EmployeerPaymentsStatisticModel paymentsStatistic = new();

    private void Payments_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SortedPaymenets));
        PaymentsStatistic.SetSource(Payments);
    }

    public void Initialize()
    {
        if (isInitialized)
        {
            throw new InvalidOperationException("already initialized");
        }

        isInitialized = true;

        Payments.CollectionChanged += Payments_CollectionChanged;
        WeakReferenceMessenger.Default.RegisterAll(this);

        OnPropertyChanged(nameof(SortedPaymenets));
        PaymentsStatistic.SetSource(Payments);
    }

    public void Receive(JobItemFieldUpdatedMessage message)
    {
        if (message.FieldName == nameof(message.Value.IsCompleted))
        {
            OnPropertyChanged(nameof(ActiveTasksCount));
        }
    }
}
