using AutoMapper;
using TodoList.WPF.ViewModels;
using TodoListApp.DataAccess.Entities;

namespace TodoList.WPF.Infrastructure.Automapper.Profiles;

public class BookToReadProfile : Profile
{
    public BookToReadProfile()
    {
        this.CreateMap<BookToReadEntity, BookToReadViewModel>();
        this.CreateMap<BookToReadViewModel, BookToReadEntity>();
    }
}
