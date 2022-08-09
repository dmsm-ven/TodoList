using AutoMapper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models;

namespace TodoList.WPF.ViewModels;

public class JobItemChangesHistoryWindowViewModel : ViewModelBase
{
    private readonly int job_item_id;

    private readonly IJobItemRepository jobItemRepository;
    private readonly IMapper mapper;

    public ICommand LoadedCommand { get; }

    public ObservableCollection<JobItemHistoryLineViewModel> HistoryItems { get; } = new();

    public JobItemChangesHistoryWindowViewModel(int job_item_id, IJobItemRepository jobItemRepository, IMapper mapper)
    {
        LoadedCommand = new LambdaCommand(async e => await Loaded());
        this.jobItemRepository = jobItemRepository;
        this.mapper = mapper;
        this.job_item_id = job_item_id;
    }

    private async Task Loaded()
    {
        var historyItems = jobItemRepository.GetHistoryChangesForJobItem(job_item_id);
        var mappedItems = mapper.Map<IEnumerable<JobItemHistoryLineViewModel>>(historyItems);

        foreach (var item in mappedItems)
        {
            HistoryItems.Add(item);
        }
    }
}
