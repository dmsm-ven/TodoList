using AutoMapper;
using System;
using System.Windows.Input;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models;
using TodoList.WPF.Services;

namespace TodoList.WPF.ViewModels;

public class AddEmployeerViewModel : ViewModelBase
{
    private readonly IEmployeerRepository repository;
    private readonly IMapper mapper;
    private readonly string default_name = "Новый работодатель";
    private readonly NavigationLocator navigationLocator;

    public event Action OnCreateNewEmployeer;
    EmployeerViewModel newEmployeer;
    
    public EmployeerViewModel NewEmployeer
    {
        get => newEmployeer;
        set => Set(ref newEmployeer, value);
    }

    public ICommand CreateCommand { get; }

    public AddEmployeerViewModel()
    {
        NewEmployeer = new EmployeerViewModel() { Name = default_name };
        CreateCommand = new LambdaCommand(Create, CanCreate);
    }
    private bool CanCreate(object arg)
    {
        return newEmployeer.Name != default_name && !string.IsNullOrWhiteSpace(NewEmployeer.Name);
    }

    public AddEmployeerViewModel(NavigationLocator navigationLocator, IEmployeerRepository repository, IMapper mapper) : this()
    {      
        this.repository = repository;
        this.navigationLocator = navigationLocator;
        this.mapper = mapper;
    }

    private void Create(object obj)
    {
        repository.AddOrUpdateEmployeer(mapper.Map<EmployeerEntity>(NewEmployeer));
        NewEmployeer = new EmployeerViewModel() { Name = default_name };
        navigationLocator.MoveTo(ViewModelType.TodoList);
    }
}
