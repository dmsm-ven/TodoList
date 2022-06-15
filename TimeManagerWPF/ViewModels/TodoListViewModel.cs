using AutoMapper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    private readonly NavigationLocator navigationLocator;
    private readonly IEmployeerRepository employeerRepository;
    private readonly IJobItemRepository jobItemRepository;
    private readonly IMapper mapper;
    public ICommand AddEmployeerCommand { get; }
    public ICommand AddJobItemCommand { get; }
    public ICommand DeleteJobItemCommand { get; }
    public ICommand MoveToTodoListCommand { get; }
    public ICommand LoadedCommand { get; }

    public ObservableCollection<TodoTabViewModel> Tabs { get; set; } = new ObservableCollection<TodoTabViewModel>();

    TodoTabViewModel selectedTab;
    public TodoTabViewModel SelectedTab
    {
        get => selectedTab;
        set => Set(ref selectedTab, value);
    }
    public TodoListViewModel()
    {
        MoveToTodoListCommand = new LambdaCommand(MoveToTodoList);
        LoadedCommand = new LambdaCommand(Loaded);
        AddEmployeerCommand = new LambdaCommand(AddEmployeer);
        AddJobItemCommand = new LambdaCommand(AddJobItem, e => SelectedTab != null);
        DeleteJobItemCommand = new LambdaCommand(DeleteJobItem, e => SelectedTab?.SelectedJobItem != null);
    }

    public TodoListViewModel(NavigationLocator navigationLocator,
        IEmployeerRepository employeerRepository, 
        IJobItemRepository jobItemRepository, 
        IMapper mapper) : this()
    {
        this.navigationLocator = navigationLocator;
        this.employeerRepository = employeerRepository;
        this.jobItemRepository = jobItemRepository;
        this.mapper = mapper;
    }
    private void MoveToTodoList(object obj)
    {
        navigationLocator.MoveTo(ViewModelType.TodoList);
    }

    private void AddEmployeer(object obj)
    {
        navigationLocator.MoveTo(ViewModelType.AddEmployeer);
    }
    private void DeleteJobItem(object obj)
    {
        var answer = MessageBox.Show("Удалить выделенное задание ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer == MessageBoxResult.Yes)
        {
            SelectedTab.DeleteJobItem();
        }
    }
    private void AddJobItem(object obj)
    {
        SelectedTab.AddJobItem();
    }
    private void Loaded(object obj)
    {
        var items = employeerRepository.GetAllEmployeer().Select(i => new TodoTabViewModel(jobItemRepository, mapper));
        SelectedTab = Tabs.FirstOrDefault();
    }
}
