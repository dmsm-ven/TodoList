using AutoMapper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TodoList.WPF.DataAccess;

namespace TodoList.WPF.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    private readonly IEmployeerRepository employeerRepository;
    private readonly IJobItemRepository jobItemRepository;
    private readonly IMapper mapper;

    public ObservableCollection<TodoTabViewModel> Tabs { get; set; } = new ObservableCollection<TodoTabViewModel>();

    TodoTabViewModel selectedTab;
    public TodoTabViewModel SelectedTab
    {
        get => selectedTab;
        set => Set(ref selectedTab, value);
    }

    public ICommand LoadedCommand { get; }

    public TodoListViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
    }

    public TodoListViewModel(IEmployeerRepository employeerRepository, IJobItemRepository jobItemRepository, IMapper mapper) : this()
    {
        this.employeerRepository = employeerRepository;
        this.jobItemRepository = jobItemRepository;
        this.mapper = mapper;
    }

    private void Loaded(object obj)
    {
        var items = employeerRepository.GetAllEmployeer().Select(i => new TodoTabViewModel(jobItemRepository, mapper));
        SelectedTab = Tabs.First();
    }
}
