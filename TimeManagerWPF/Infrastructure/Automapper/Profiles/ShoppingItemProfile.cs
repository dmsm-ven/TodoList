using AutoMapper;
using TodoList.WPF.Models.ShoppingItem;
using TodoList.WPF.ViewModels;
using TodoListApp.DataAccess.Entities;

namespace TodoList.WPF.Infrastructure.Automapper.Profiles;

public class ShoppingItemProfile : Profile
{
    public ShoppingItemProfile()
    {
        CreateMap<ShoppingItemViewModel, ShoppingItemEntity>();
        CreateMap<ShoppingItemEntity, ShoppingItemViewModel>();

        CreateMap<ShoppingItemCategoryEntity, ShoppingItemCategory>();
        CreateMap<ShoppingItemCategory, ShoppingItemCategoryEntity>();
    }
}
