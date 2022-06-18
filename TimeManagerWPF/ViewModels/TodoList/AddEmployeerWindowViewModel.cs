using AutoMapper;
using System.Windows;
using System.Windows.Input;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess;
using TodoList.WPF.Models;

namespace TodoList.WPF.ViewModels;

public class AddEmployeerWindowViewModel : ViewModelBase
{
    private readonly IEmployeerRepository repository;
    private readonly IMapper mapper;
    private readonly string default_name = "Новый работодатель";

    EmployeerViewModel newEmployeer;
    
    public EmployeerViewModel NewEmployeer
    {
        get => newEmployeer;
        set => Set(ref newEmployeer, value);
    }

    public ICommand CreateCommand { get; }

    public AddEmployeerWindowViewModel()
    {
        NewEmployeer = new EmployeerViewModel() { Name = default_name };
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
