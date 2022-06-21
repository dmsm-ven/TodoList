using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models;
using TodoList.WPF.Views;

namespace TodoList.WPF.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    private readonly NavigationLocator navigationLocator;
    private readonly IEmployeerRepository employeerRepository;
    private readonly IEmployeerPaymentRepository employeerPaymentRepository;
    private readonly IJobItemRepository jobItemRepository;
    private readonly IMapper mapper;
    private readonly IHost host;

    bool isLoading;
    public bool IsLoading { get { return isLoading; } set => Set(ref isLoading, value); }

    public ICommand AddNewEmployeerTabCommand { get; }
    public ICommand LoadedCommand { get; }
    public ObservableCollection<TodoListTabViewModel> Tabs { get; set; } = new ObservableCollection<TodoListTabViewModel>();

    TodoListTabViewModel selectedTab;
    public TodoListTabViewModel SelectedTab
    {
        get => selectedTab;
        set
        {
            if (Set(ref selectedTab, value))
            {
                selectedTab?.LoadedCommand.Execute(null);
            }
        }
    }
    public TodoListViewModel()
    {
        AddNewEmployeerTabCommand = new LambdaCommand(AddNewEmployeerTab);
        LoadedCommand = new LambdaCommand(Loaded);
        Tabs = new ObservableCollection<TodoListTabViewModel>()
        {
            new TodoListTabViewModel() { Employeer = new EmployeerViewModel(0, "Tab 1") },
            new TodoListTabViewModel() { Employeer = new EmployeerViewModel(0, "Tab 2") },
            new TodoListTabViewModel() { Employeer = new EmployeerViewModel(0, "Tab 3") }
        };
    }
    public TodoListViewModel(NavigationLocator navigationLocator,
        IEmployeerRepository employeerRepository,
        IEmployeerPaymentRepository employeerPaymentRepository, 
        IJobItemRepository jobItemRepository, 
        IMapper mapper,
        IHost host) : this()
    {
        this.navigationLocator = navigationLocator;
        this.employeerRepository = employeerRepository;
        this.employeerPaymentRepository = employeerPaymentRepository;
        this.jobItemRepository = jobItemRepository;
        this.mapper = mapper;
        this.host = host;
    }


    private void AddNewEmployeerTab(object obj)
    {
        var window = host.Services.GetRequiredService<AddEmployeerWindow>();
        window.DataContext = host.Services.GetRequiredService(typeof(AddEmployeerWindowViewModel));
        if (window.ShowDialog() == true)
        {
            Loaded(null);
;        }
    }
    private async void Loaded(object obj)
    {
        Tabs.Clear();

        IsLoading = true;

        await Task.Delay(TimeSpan.FromSeconds(0.25));

        var employeers = await Task.Run(() => employeerRepository.GetAllEmployeer());

        employeers
            .Select(emp => new EmployeerViewModel(emp.Id, emp.Name, mapper, jobItemRepository, employeerPaymentRepository))
            .Select(i => new TodoListTabViewModel(i, jobItemRepository, employeerPaymentRepository, mapper))
            .ToList()
            .ForEach(t => Tabs.Add(t));

        IsLoading = false;

        if (SelectedTab == null)
        {
            SelectedTab = Tabs.FirstOrDefault();
        }
    }
}
