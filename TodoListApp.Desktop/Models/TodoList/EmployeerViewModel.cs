
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TodoListApp.Desktop.Models.Messages;
using TodoListApp.Desktop.ViewModels;

namespace TodoListApp.Desktop.Models.TodoList;

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

    private const int InitialPaymentsCount = 10;

    /// <summary>true после нажатия "Показать еще" - в списке оплат выводятся все записи, а не последние 10</summary>
    [NotifyPropertyChangedFor(nameof(SortedPayments))]
    [NotifyPropertyChangedFor(nameof(HasHiddenPayments))]
    [ObservableProperty]
    private bool showAllPayments;

    public IEnumerable<EmployeerPaymentViewModel> SortedPayments
    {
        get
        {
            var ordered = Payments.OrderByDescending(x => x.TransferArrivalDate);
            return (ShowAllPayments ? ordered : ordered.Take(InitialPaymentsCount)).ToArray();
        }
    }

    public bool HasHiddenPayments => !ShowAllPayments && Payments.Count > InitialPaymentsCount;

    [CommunityToolkit.Mvvm.Input.RelayCommand]
    private void ShowMorePayments()
    {
        ShowAllPayments = true;
    }

    [ObservableProperty]
    private ObservableCollection<JobItemViewModel> todoItems = new();

    [ObservableProperty]
    private EmployeerPaymentsStatisticModel paymentsStatistic = new();

    private void Payments_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SortedPayments));
        OnPropertyChanged(nameof(HasHiddenPayments));
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

        OnPropertyChanged(nameof(SortedPayments));
        PaymentsStatistic.SetSource(Payments);
    }

    public void Receive(JobItemFieldUpdatedMessage message)
    {
        OnPropertyChanged(nameof(ActiveTasksCount));
    }
}
