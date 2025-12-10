using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;

namespace TodoList.WPF.ViewModels;

public record EmployeerCreatedMessage(string newEmployeerName);

public partial class AddEmployeerWindowViewModel() : ObservableObject
{
    private static string default_name = "Новый работодатель";

    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    [ObservableProperty]
    private string newEmployeerName = default_name;

    private bool CanCreate()
    {
        return NewEmployeerName != default_name && !string.IsNullOrWhiteSpace(NewEmployeerName);
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private void Create(Window window)
    {
        WeakReferenceMessenger.Default.Send(new EmployeerCreatedMessage(newEmployeerName));

        window.DialogResult = true;
    }
}
