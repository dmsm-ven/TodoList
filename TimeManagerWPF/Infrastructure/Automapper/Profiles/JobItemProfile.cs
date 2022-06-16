using AutoMapper;
using TodoList.DataAccess;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Automapper.Profiles;

internal class JobItemProfile : Profile
{
    public JobItemProfile()
    {
        CreateMap<JobItemEntity, JobItemViewModel>();
        CreateMap<JobItemViewModel, JobItemEntity>();
    }
}
