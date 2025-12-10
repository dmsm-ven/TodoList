using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoListApp.Desktop.ViewModels.Windows;

public partial class ConnectionErrorWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string errorMessage;
}
