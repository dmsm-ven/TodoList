using AutoMapper;
using TodoList.DataAccess;
using TodoList.WPF.Models;

namespace TodoList.WPF.Automapper.Profiles;

public class EmployeerProfile : Profile
{
    public EmployeerProfile()
    {
        CreateMap<EmployeerViewModel, EmployeerEntity>();
        CreateMap<EmployeerEntity, EmployeerViewModel>();
    }
}
