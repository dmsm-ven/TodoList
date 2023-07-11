using AutoMapper;
using System.Windows;
using System.Windows.Input;
using TodoList.WPF.Models.TodoList;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoList.WPF.ViewModels;

public class AddEmployeerWindowViewModel : ViewModelBase
{
    private readonly IEmployeerRepository repository;
    private readonly IMapper mapper;
    private readonly string default_name = "Новый работодатель";
    private EmployeerViewModel newEmployeer;

    public EmployeerViewModel NewEmployeer
    {
        get => newEmployeer;
        set => Set(ref newEmployeer, value);
    }

    public ICommand CreateCommand { get; }

    public AddEmployeerWindowViewModel()
    {
        NewEmployeer = new EmployeerViewModel(0, default_name);
        CreateCommand = new LambdaCommand(Create, CanCreate);
    }

    public AddEmployeerWindowViewModel(IEmployeerRepository repository, IMapper mapper) : this()
    {
        this.repository = repository;
        this.mapper = mapper;
    }
    private bool CanCreate(object arg)
    {
        return newEmployeer.Name != default_name && !string.IsNullOrWhiteSpace(NewEmployeer.Name);
    }

    private void Create(object obj)
    {
        repository.AddOrUpdateEmployeer(mapper.Map<EmployeerEntity>(NewEmployeer));
        (obj as Window).DialogResult = true;
    }
}
