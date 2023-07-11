using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoList.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IEmployeerRepository employeerRepository;
    private readonly IJobItemRepository jobItemRepository;
    private readonly IEmployeerPaymentRepository employeerPaymentRepository;

    public MainViewModel(IEmployeerRepository employeerRepository,
        IEmployeerPaymentRepository employeerPaymentRepository,
        IJobItemRepository jobItemRepository) : this()
    {
        this.employeerRepository = employeerRepository;
        this.employeerPaymentRepository = employeerPaymentRepository;
        this.jobItemRepository = jobItemRepository;
    }

    public ObservableCollection<EmployeerEntity> Employeers { get; }
    public ObservableCollection<JobItemEntity> JobItems { get; }

    private EmployeerEntity? selectedEmployeer;
    public EmployeerEntity? SelectedEmployeer
    {
        get => selectedEmployeer;
        set
        {
            if (this.RaiseAndSetIfChanged(ref selectedEmployeer, value) != null)
            {
                Task.Run(LoadJobItemsForCurrentEmployeer);
            }
        }
    }
    public ReactiveCommand<EmployeerEntity, Unit> ChangeSelectedEmployeerCommand { get; }

    private readonly Dictionary<int, List<JobItemEntity>> jobItemsCache;

    public string CurrentEmployeerUnpaidCompletedTotalSum
    {
        get
        {
            decimal total = 0;
            if (selectedEmployeer != null)
            {
                total = jobItemsCache[selectedEmployeer.id]
                    .Where(j => j.is_completed && !j
                    .is_payed).Sum(j => j.price);
            }
            return $"{total:F0} руб.";
        }
    }

    public MainViewModel()
    {
        JobItems = new ObservableCollection<JobItemEntity>();
        Employeers = new ObservableCollection<EmployeerEntity>();
        jobItemsCache = new Dictionary<int, List<JobItemEntity>>();
        ChangeSelectedEmployeerCommand = ReactiveCommand.Create<EmployeerEntity>((newEmployeer) => SelectedEmployeer = newEmployeer);
    }

    public async Task LoadedCommand()
    {
        var emp = await Task.Run(() => employeerRepository.GetAllEmployeer());

        foreach (var e in emp)
        {
            Employeers.Add(e);
        }

        SelectedEmployeer = Employeers.FirstOrDefault();
    }

    private async Task LoadJobItemsForCurrentEmployeer()
    {
        if (!jobItemsCache.TryGetValue(selectedEmployeer.id, out var jobItems))
        {
            var jobItemsRemote = await Task.Run(() => jobItemRepository.GetAllJobItems(selectedEmployeer.id));
            jobItemsCache[selectedEmployeer.id] = jobItemsRemote.OrderByDescending(i => i.start_date).ToList();
        }

        Dispatcher.UIThread.Invoke(() =>
        {
            if (JobItems.Count > 0)
            {
                JobItems.Clear();
            }
            JobItems.AddRange(jobItemsCache[selectedEmployeer.id]);

            this.RaisePropertyChanged(nameof(CurrentEmployeerUnpaidCompletedTotalSum));
        });
    }
}
