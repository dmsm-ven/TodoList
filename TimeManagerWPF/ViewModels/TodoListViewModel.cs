using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    private readonly NavigationLocator navigationLocator;
    private readonly IEmployeerRepository employeerRepository;
    private readonly IEmployeerPaymentRepository employeerPaymentRepository;
    private readonly IJobItemRepository jobItemRepository;
    private readonly IMapper mapper;
    public ICommand AddEmployeerCommand { get; }
    public ICommand MoveToTodoListCommand { get; }
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
        LoadedCommand = new LambdaCommand(Loaded);
        Tabs = new ObservableCollection<TodoListTabViewModel>();
        MoveToTodoListCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.TodoList));        
        AddEmployeerCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.AddEmployeer));     
    }

    public TodoListViewModel(NavigationLocator navigationLocator,
        IEmployeerRepository employeerRepository,
        IEmployeerPaymentRepository employeerPaymentRepository, 
        IJobItemRepository jobItemRepository, 
        IMapper mapper) : this()
    {
        this.navigationLocator = navigationLocator;
        this.employeerRepository = employeerRepository;
        this.employeerPaymentRepository = employeerPaymentRepository;
        this.jobItemRepository = jobItemRepository;
        this.mapper = mapper;
    }

    private void Loaded(object obj)
    {
        Tabs.Clear();

        employeerRepository
            .GetAllEmployeer()
            .Select(emp => new EmployeerViewModel()
            {
                Name = emp.Name,
                Id = emp.Id,
                TodoItems = new ObservableCollection<JobItemViewModel>(mapper.Map<IEnumerable<JobItemViewModel>>(jobItemRepository.GetAllJobItems(emp.Id))),
                Payments = new ObservableCollection<EmployeerPaymentViewModel>(mapper.Map<IEnumerable<EmployeerPaymentViewModel>>(employeerPaymentRepository.GetAllPaymentsForEmployeer(emp.Id)))
            })
            .Select(i => new TodoListTabViewModel(i, jobItemRepository, employeerPaymentRepository, mapper))
            .ToList()
            .ForEach(t => Tabs.Add(t));
        if (SelectedTab == null)
        {
            SelectedTab = Tabs.FirstOrDefault();
        }
    }
}
