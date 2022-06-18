using AutoMapper;
using TodoList.WPF.DataAccess.Entities;
using TodoList.WPF.ViewModels;

namespace TodoList.WPF.Infrastructure.Automapper.Profiles;

public class BookToReadProfile : Profile
{
    public BookToReadProfile()
    {
        this.CreateMap<BookToReadEntity, BookToReadViewModel>();
        this.CreateMap<BookToReadViewModel, BookToReadEntity>();
    }
}
