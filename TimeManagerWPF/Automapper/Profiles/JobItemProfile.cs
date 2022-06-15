using AutoMapper;
using TodoList.DataAccess;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Automapper.Profiles;

internal class JobItemProfile : Profile
{
    public JobItemProfile()
    {
        this.CreateMap<JobItemEntity, JobItemViewModel>();
        this.CreateMap<JobItemViewModel, JobItemEntity>();
    }
}
