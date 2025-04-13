using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoList.WPF.ViewModels.Windows;

public partial class ConnectionErrorWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string errorMessage;
}
