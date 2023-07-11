using AutoMapper;
using TodoList.WPF.Models;
using TodoList.WPF.Models.TodoList;
using TodoListApp.DataAccess.Entities;

namespace TodoList.WPF.Infrastructure.Automapper.Profiles;

public class EmployeerProfile : Profile
{
    public EmployeerProfile()
    {
        CreateMap<EmployeerViewModel, EmployeerEntity>()
            .ForMember(x => x.id, x => x.MapFrom(e => e.Id))
            .ForMember(x => x.name, x => x.MapFrom(e => e.Name));

        CreateMap<EmployeerEntity, EmployeerViewModel>()
            .ForMember(x => x.Id, x => x.MapFrom(e => e.id))
            .ForMember(x => x.Name, x => x.MapFrom(e => e.name));

        CreateMap<EmployeerPaymentEntity, EmployeerPaymentViewModel>()
            .ForMember(x => x.Id, x => x.MapFrom(p => p.id))
            .ForMember(x => x.Amount, x => x.MapFrom(p => p.amount))
            .ForMember(x => x.EmployeerId, x => x.MapFrom(p => p.employeer_id))
            .ForMember(x => x.TransferArrivalDate, x => x.MapFrom(p => p.transfer_arrival_date));


        CreateMap<EmployeerPaymentViewModel, EmployeerPaymentEntity>()
            .ForMember(x => x.id, x => x.MapFrom(p => p.Id))
            .ForMember(x => x.amount, x => x.MapFrom(p => p.Amount))
            .ForMember(x => x.employeer_id, x => x.MapFrom(p => p.EmployeerId))
            .ForMember(x => x.transfer_arrival_date, x => x.MapFrom(p => p.TransferArrivalDate));


    }
}
