using AutoMapper;
using TodoList.DataAccess;
using TodoList.WPF.DataAccess.Entities;
using TodoList.WPF.Models;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Automapper.Profiles;

internal class JobItemProfile : Profile
{
    public JobItemProfile()
    {
        CreateMap<JobItemEntity, JobItemViewModel>();
        CreateMap<JobItemViewModel, JobItemEntity>();

        CreateMap<JobItemHistoryEntity, JobItemHistoryLineViewModel>()
            .ForMember(x => x.ChangedPropertyName, x => x.MapFrom(p => p.property_name))
            .ForMember(x => x.DateTime, x => x.MapFrom(p => p.date_time))
            .ForMember(x => x.NewValue, x => x.MapFrom(p => p.new_value));
    }
}
