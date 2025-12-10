using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TodoListApp.Core.Models;

namespace TodoListApp.Desktop.ViewModels.Windows;

public partial class JobItemChangesHistoryWindowViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<JobItemHistoryLineModel> historyItems = new();

}
