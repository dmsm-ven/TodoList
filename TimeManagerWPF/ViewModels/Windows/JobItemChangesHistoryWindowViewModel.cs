using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TodoListApp.Core.Models;

namespace TodoList.WPF.ViewModels.Windows;

public partial class JobItemChangesHistoryWindowViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<JobItemHistoryLineModel> historyItems = new();

    [RelayCommand]
    private async Task Loaded()
    {
        throw new NotImplementedException();
        /*
        var historyItems = jobItemRepository.GetHistoryChangesForJobItem(job_item_id);

        foreach (var item in historyItems.Select(i => i.ToModel()))
        {
            HistoryItems.Add(item);
        }
        */
    }
}
