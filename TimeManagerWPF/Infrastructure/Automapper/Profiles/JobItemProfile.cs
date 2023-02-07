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
        CreateMap<JobItemEntity, JobItemViewModel>()
            .ForMember(x => x.StartDate, x => x.MapFrom(i => i.start_date))
            .ForMember(x => x.EndDate, x => x.MapFrom(i => i.end_date))
            .ForMember(x => x.IsCompleted, x => x.MapFrom(i => i.is_completed))
            .ForMember(x => x.IsPayed, x => x.MapFrom(i => i.is_payed))
            .ForMember(x => x.Id, x => x.MapFrom(i => i.id))
            .ForMember(x => x.Title, x => x.MapFrom(i => i.title))
            .ForMember(x => x.Description, x => x.MapFrom(i => i.description))
            .ForMember(x => x.EmployeerId, x => x.MapFrom(i => i.employeer_id));

        CreateMap<JobItemViewModel, JobItemEntity>()
            .ForMember(x => x.start_date, x => x.MapFrom(i => i.StartDate))
            .ForMember(x => x.end_date, x => x.MapFrom(i => i.EndDate))
            .ForMember(x => x.is_completed, x => x.MapFrom(i => i.IsCompleted))
            .ForMember(x => x.is_payed, x => x.MapFrom(i => i.IsPayed))
            .ForMember(x => x.id, x => x.MapFrom(i => i.Id))
            .ForMember(x => x.title, x => x.MapFrom(i => i.Title))
            .ForMember(x => x.description, x => x.MapFrom(i => i.Description))
            .ForMember(x => x.employeer_id, x => x.MapFrom(i => i.EmployeerId));

        CreateMap<JobItemHistoryEntity, JobItemHistoryLineViewModel>()
            .ForMember(x => x.ChangedPropertyName, x => x.MapFrom(p => p.property_name))
            .ForMember(x => x.DateTime, x => x.MapFrom(p => p.date_time))
            .ForMember(x => x.NewValue, x => x.MapFrom(p => p.new_value));
    }
}
